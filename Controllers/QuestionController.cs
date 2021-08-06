using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishVocabularyTest.Models;
using EnglishVocabularyTest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace EnglishVocabularyTest.Controllers
{
    public class QuestionController : Controller
    {
        private readonly Word[] words;
        public QuestionController()
        {
            words = Operation.words;
        }

        [HttpGet]
        public IActionResult QuestionPage(QuestionVM qvm)
        {
            if (qvm.Counter == 40)
                return RedirectToAction("ResultPage", "Question", new { result = qvm.Rating });

            qvm = Operation.GetQuestion(Operation.GetScope(qvm), qvm);
            qvm.Answers = Operation.GetAnswers(qvm);
            return View(qvm);
        }

        [HttpPost]
        public IActionResult QuestionPage(QuestionVM respond, string Answer)
        {
            respond = respond.Counter < 30 ?
                Operation.ChangeRating(respond, Answer, 1000) : Operation.ChangeRating(respond, Answer, 100);

            respond.Counter++;
            return QuestionPage(respond);
        }

        public IActionResult ResultPage(int result)
        {
            ViewBag.Result = result / 100 * 100;
            return View();
        }
    }
}