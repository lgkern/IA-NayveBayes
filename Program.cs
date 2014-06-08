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
                FoldCrossValidation(i*20 + 27);
            }


            Console.ReadKey();

        }

        private void FoldCrossValidation(int startIndex)
        {
            //Cria conjuntos de treinamento e de teste para essa iteração
            List<int> testingSet = new List<int>();
            List<int> trainingSet = new List<int>();
            int distance = 0;
            for (int i = 27; i < 227; i++)
            {
                distance =  i - startIndex;
                if(distance < 0 || distance > 20)
                {
                    trainingSet.Add(i);                    
                    continue;
                }
                testingSet.Add(i);            
            }



        }       

        FileHandler[] positive;
        FileHandler[] negative;

    }

    class Dictionary
    {
        public Dictionary()
        {

        }

        private int _wordCount = 0;
        private List<Word> words;

        public void setWordCount(int wordCount)
        {
            _wordCount = wordCount;
        }

        public int addWord(String word)
        {            
            if (!words.Contains(new Word(word)))
            {         
                words.Add(new Word(word));
            }
            int foundIndex = words.FindIndex(x => x.Text() == word);

            return words[foundIndex].AddReference(); ;
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
        List<String> purifiedList;
        
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
