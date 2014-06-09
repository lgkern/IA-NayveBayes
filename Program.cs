using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IA_Nayve_Bayes
{
    class Program
    {        
        static void Main(string[] args)
        {
            NayveBayes scenario = new NayveBayes();
        }
    }
    class NayveBayes
    {
        Dictionary dictionary;        
        public NayveBayes()
        {
            //Tokenization
            positive = new FileHandler[200];
            negative = new FileHandler[200];

            for (int i = 0; i < 200; i++)
            {
                positive[i] = new FileHandler(@"G:\UFRGS\IA\positivo\" + (i + 27) + ".txt");
                negative[i] = new FileHandler(@"G:\UFRGS\IA\negativo\" + (i + 27) + ".txt");
            }

            //10-fold cross validation *10
            for (int i = 0; i < 10; i++)
            {
                FoldCrossValidation(i*20);
            }


            Console.ReadKey();

        }

        private void FoldCrossValidation(int startIndex)
        {
            dictionary = new Dictionary();
            
            //Cria conjuntos de treinamento e de teste para essa iteração
            List<int> testingSet = new List<int>();
            List<int> trainingSet = new List<int>();
            int distance = 0;
            for (int i = 0; i < 200; i++)
            {
                distance =  i - startIndex;
                if(distance < 0 || distance > 20)
                {
                    trainingSet.Add(i);                    
                    continue;
                }
                testingSet.Add(i);            
            }

            foreach (int index in trainingSet)
            {
                Console.WriteLine("Creating Dictionary from file " + (index+27));
                positiveTraining(index);
                negativeTraining(index);
            }
            //Done with the gym, now to some productory action!
            int test = 0;
            positiveResults = new bool[20];
            negativeResults = new bool[20];
            foreach (int index in testingSet)
            {
                Console.WriteLine("Testing files " + (index + 27) + ".txt");
                positiveResults[test] = positiveClassifier(index);
                negativeResults[test] = negativeClassifier(index);
                test++;
            }
        }       

        private bool positiveClassifier(int index)
        {
            double positiveAc = 1.0;
            double negativeAc = 1.0;
            foreach (String word in positive[index].purifiedList)
            {
                positiveAc *= dictionary.PositiveProbability(word);
                negativeAc *= dictionary.NegativeProbability(word);
            }
            return positiveAc>negativeAc?true:false;
        }

        private bool negativeClassifier(int index)
        {
            double positiveAc = 1.0;
            double negativeAc = 1.0;
            foreach (String word in negative[index].purifiedList)
            {
                positiveAc *= dictionary.PositiveProbability(word);
                negativeAc *= dictionary.NegativeProbability(word);
            }
            return positiveAc > negativeAc ? true : false;
        }

        private void positiveTraining(int index)
        {
            foreach (String word in positive[index].purifiedList)
            {
                dictionary.addPositiveWord(word);
            }
        }

        private void negativeTraining(int index)
        {
            foreach (String word in negative[index].purifiedList)
            {
                dictionary.addNegativeWord(word);
            }
        }

        FileHandler[] positive;
        FileHandler[] negative;

        bool[]  positiveResults;
        bool[]  negativeResults;

    }

    class Dictionary
    {
        public Dictionary()
        {
            positiveWords = new List<Word>();
            negativeWords = new List<Word>();
            _wordCount = 0;
            _totalWords = 0;
        }

        private int _wordCount = 0;
        private int _totalWords = 0;
        private List<Word> positiveWords;
        private List<Word> negativeWords;

        public void setWordCount(int wordCount)
        {
            _wordCount = wordCount;
        }

        //Probabilidade da palavra ser dessa classe é (Count+1)/(Vocabulário+Wordcount)

        public double PositiveProbability(String word)
        {
            int bullshit = _wordCount + _totalWords;

            if (!positiveWords.Contains(new Word(word)))
            {
                return (double)1 / bullshit;
            }
            int foundIndex = positiveWords.FindIndex(x => x.Text() == word);
            return (double)(1 + positiveWords[foundIndex].ReferenceCount()) / bullshit;
        }

        public double NegativeProbability(String word)
        {
            int bullshit = _wordCount + _totalWords;

            if (!negativeWords.Contains(new Word(word)))
            {
                return (double)1 / bullshit;
            }
            int foundIndex = negativeWords.FindIndex(x => x.Text() == word);
            return (double)(1 + negativeWords[foundIndex].ReferenceCount()) / bullshit;
        }

        public int addPositiveWord(String word)
        {
            _totalWords++;
            if (!positiveWords.Contains(new Word(word)))
            {
                positiveWords.Add(new Word(word));
                if (!negativeWords.Contains(new Word(word)))
                    _wordCount++;
            }
            int foundIndex = positiveWords.FindIndex(x => x.Text() == word);

            return positiveWords[foundIndex].AddReference();
        }

        public int addNegativeWord(String word)
        {
            _totalWords++;
            if (!negativeWords.Contains(new Word(word)))
            {
                negativeWords.Add(new Word(word));
                if (!positiveWords.Contains(new Word(word)))
                    _wordCount++;
            }
            int foundIndex = negativeWords.FindIndex(x => x.Text() == word);

            return negativeWords[foundIndex].AddReference();
        }
    }

    class Word
    {
        public Word(String word)
        {
            _word = word;            
        }

        public String _word;
        private int references = 0;

        public String Text()
        {
            return _word;
        }

        public int AddReference()
        {
            references++;
            return references;
        }

        public int ReferenceCount()
        {
            return references;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Word objAsWord = obj as Word;
            if (objAsWord == null) return false;
            else return Equals(objAsWord);
        }

        public bool Equals(Word other)
        {
            if (other == null) return false;
            return (this._word.Equals(other._word));
        }
    }

    class FileHandler
    {
        public FileHandler(string fileName)
        {
            text = new StreamReader(fileName);
            removeImpurities();
        }
        StreamReader text;
        String[] purifiedText;
        public List<String> purifiedList;
        
        void removeImpurities()
        {   
            String currentLine;           
            purifiedList = new List<String>();
            while((currentLine = text.ReadLine()) != null)
            {
                currentLine = currentLine.ToLower();
                purifiedText = currentLine.Split(new string[]{" ",".",",",";","-","(",")","\\","/","\"",":"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (String word in purifiedText)
                {
                    if(!commonWords.Contains(word))
                    {
                        purifiedList.Add(word);
                    }
                }                
            }
        }

        string[] commonWords = {"a","as","o","os","de","da","das","do","dos","um","uns","uma","umas","é","são","foi","foram","para",
                                         "isso","isto","esse","esses","este","estes","ou","e","ser","por","...","não","num","em","que"};

    }
}
