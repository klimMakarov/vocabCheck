using EnglishVocabularyTest.Models;
using EnglishVocabularyTest.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EnglishVocabularyTest
{
    //Тип для проведения промежуточных операций
    public static class Operation
    {
        static Random rnd = new Random();

        public static Word[] words = GetWordArray();

        //Создать коллекцию из всех слов
        private static Word[] GetWordArray()
        {
            using (var reader = File.OpenText("wwwroot/fList/20200.json"))
            {
                return JsonConvert.DeserializeObject<Word[]>(reader.ReadToEnd());
            }
        }

        //Определить промежуток, из которого можно взять следующее слово
        //Например, если возвращается 4000, то следующее слово будет взято из промежутка от 4000 до 5000
        public static int GetScope(QuestionVM vm)
        {
            if (vm.Counter < 30)
                return vm.Counter == 0 ? 5000 : vm.Rating / 1000 * 1000;

            if (vm.Counter == 30)
                return vm.Rating / 1000 * 1000 + 500;

            return vm.Rating / 100 * 100;
        }

        //Возвращает следующее слово для юзера
        public static QuestionVM GetQuestion(int scope, QuestionVM qvm)
        {
            Word _word = null;

            if (qvm.PreviousWords == null)
                qvm.PreviousWords = new List<string>();

            while (_word == null)
            {
                try
                {
                    if (qvm.Counter < 30)
                        _word = words.Where(x => x.Rating == rnd.Next(scope, scope + 1000)).First();
                    if (qvm.Counter == 30 || qvm.Counter > 30)
                        _word = words.Where(x => x.Rating == rnd.Next(scope, scope + 100)).First();
                    //    _word = words.Where(x => x.Rating == scope).First();
                    //if (qvm.Counter > 30)
                    //    _word = words.Where(x => x.Rating == rnd.Next(scope, scope + 100)).First();
                    if (qvm.PreviousWords.Contains(_word.Eng))
                        throw new Exception();
                }
                catch
                {
                    _word = null;
                    //scope++;
                }
            }

            qvm.Question = _word.Eng;
            qvm.RightAnswer = _word.Rus;
            qvm.Rating = _word.Rating;
            qvm.PreviousWords.Add(qvm.Question);
            return qvm;
        }

        // Возвращает варианты ответа, среди которых один - правильный
        public static string[] GetAnswers(QuestionVM question)
        {
            string[] answers = new string[5];
            string answer;
            for (int i = 0; i < 5; i++)
            {
                do
                    answer = words[rnd.Next(3000, 10000)].Rus;
                while (answer == question.RightAnswer || answers.Contains(answer));
                answers[i] = answer;
            }
            answers[rnd.Next(0, 4)] = question.RightAnswer;
            return answers;
        }

        // В зависимости от того, дал ли юзер нужное кол-во правильных / неправильных ответов, 
        // меняется область, из которой будет браться следующее слово
        public static QuestionVM ChangeRating(QuestionVM respond, string Answer, int points)
        {
            if (Answer == respond.RightAnswer)
            {
                respond.Scale++;
                if (respond.Scale == 2)
                {
                    respond.Scale = 0;
                    respond.Rating += points;
                }
                return respond;
            }
            else
            {
                respond.Scale--;
                if (respond.Scale == -2)
                {
                    respond.Scale = 0;
                    respond.Rating -= points;
                }
                return respond;
            }
        }
    }
}
