using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheRightDecision.Data;
using TheRightDecision.Models;
using TheRightDecision.Models.ViewModels;

namespace TheRightDecision.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Coagulation
        public IActionResult CoagulationResult()
        {
            ViewBag.Criteria = _context.Criteria;
            IEnumerable<ResultElement> coagulationResultList = Coagulate();
            ResultViewModel coagulationResult = new ResultViewModel()
            {
                ResultList = coagulationResultList.OrderByDescending(c => c.Weight),
                Criteria = _context.Criteria
            };

            SaveCoagulationResult(coagulationResult.ResultList.ToList());

            return View(coagulationResult);
        }


        private List<ResultElement> Coagulate()
        {
            List<ResultElement> result = new List<ResultElement>();

            List<Alternative> alternatives = _context.Alternatives
                .Include(a => a.Vectors)
                    .ThenInclude(v => v.Mark)
                .ToList();

            int numberOfCriteria = _context.Criteria.Count();

            foreach (var a in alternatives)
            {
                ResultElement resultElement = CountWeight(a);
                result.Add(resultElement);
            }

            double maxResultWeight = result.Max(r => r.Weight);
            foreach (var resultElement in result.Where(r => r.Weight == maxResultWeight))
            {
                resultElement.IsOneOfBest = true;
            }
            return result;
        }

        private ResultElement CountWeight(Alternative alternative)
        {
            ResultElement resultElement = new ResultElement
            {
                Alternative = alternative,
                Weight = 0
            };
            foreach (var vector in alternative.Vectors)
            {
                double markValue = (double)vector.Mark.Normalized;
                double normalizationAlphaDenominator = (double)_context.Vectors.Where(v => v.Mark.CriterionId == vector.Mark.CriterionId)
                    .Sum(v => v.Mark.Normalized);

                resultElement.Weight += markValue / normalizationAlphaDenominator;
            }

            resultElement.Weight = Math.Round(resultElement.Weight, 2);
            return resultElement;
        }

        private void SaveCoagulationResult(List<ResultElement> resultElements)
        {
            RemoveResultsByPerson();

            List<Result> results = new List<Result>();
            int previousRank = 1;
            double previousValue = resultElements[0].Weight;
            foreach (ResultElement el in resultElements)
            {
                Result newResult = new Result()
                {
                    Alternative = el.Alternative,
                    AlternativeWeight = el.Weight,
                };
                if (el.Weight != previousValue)
                {
                    previousRank++;
                    previousValue = el.Weight;
                }
                newResult.Rank = previousRank;

                results.Add(newResult);
            }

            _context.Results.AddRange(results);
            _context.SaveChanges();
        }

        private void RemoveResultsByPerson()
        {
            // TODO: Implement for not-null users.
            List<Result> results = _context.Results.Where(r => !r.PersonId.HasValue).ToList();
            _context.RemoveRange(results);
            _context.SaveChanges();
        }
        #endregion

        #region Sport comparison

        public IActionResult SportComparison()
        {
            return RedirectToAction("SportComparisonVoting");
        }

        public IActionResult SportComparisonVoting()
        {
            var model = new SportComparisonVotingViewModel();
            IEnumerable<Alternative> alternatives = _context.Alternatives
                .Include(a => a.Vectors)
                    .ThenInclude(v => v.Mark)
                        .ThenInclude(m => m.Criterion);

            model.Alternatives = alternatives.Select(a => new SportComparisonVotingElement()
            {
                AlternativeId = a.AlternativeId,
                AlternativeName = a.Name,
                Marks = a.Vectors
                        .OrderBy(v => v.Mark.CriterionId)
                        .ToDictionary(
                            v => v.Mark.Criterion.Name,
                            v =>
                            {
                                if (string.IsNullOrEmpty(v.Mark.Criterion.Units))
                                {
                                    return v.Mark.Name;
                                }
                                return v.Mark.Name + " " + v.Mark.Criterion.Units;
                            })
            });

            return View(model);
        }

        #endregion

        #region Bord's rule
        public IActionResult BordGroupResult()
        {
            GroupVoteResultViewModel model = new GroupVoteResultViewModel()
            {
                Details = new List<GroupVoteResultModelElement>()
            };

            List<Person> people = _context.People
                .Include(p => p.Results)
                    .ThenInclude(r => r.Alternative)
                .ToList();

            Dictionary<Alternative, int> pointsList = _context.Alternatives.ToDictionary(a => a, a => 0);

            foreach (var p in people)
            {
                GroupVoteResultModelElement detail = new GroupVoteResultModelElement
                {
                    PersonName = p.Name,
                    SelectedAlternatives = p.Results
                        .OrderBy(r => r.Rank)
                        .Select(r => r.Alternative.Name)
                        .ToList()
                };
                int maxPoints = p.Results.Count;
                foreach (var res in p.Results)
                {
                    pointsList[res.Alternative] += maxPoints - res.Rank;
                }
                model.Details.Add(detail);
            }

            model.Points = pointsList.Max(el => el.Value);
            model.WinnerName = pointsList.First(el => el.Value == model.Points).Key.Name;

            return View(model);
        }

        #endregion

        #region Kopler's rule
        public IActionResult KoplerGroupResult()
        {
            GroupVoteResultViewModel model = new GroupVoteResultViewModel()
            {
                Details = new List<GroupVoteResultModelElement>()
            };

            List<Person> people = _context.People
                .Include(p => p.Results)
                    .ThenInclude(r => r.Alternative)
                .ToList();

            List<Alternative> alternatives = _context.Alternatives.ToList();
            Dictionary<int, int> pointsDictionary = alternatives.ToDictionary(a => a.AlternativeId, a => 0);

            int numberOfAlternatives = alternatives.Count;
            for (int leftIndex = 0; leftIndex < numberOfAlternatives - 1; leftIndex++)
            {
                int leftAlternativeId = alternatives[leftIndex].AlternativeId;
                for (int rightIndex = leftIndex + 1; rightIndex < numberOfAlternatives; rightIndex++)
                {
                    int rightAlternativeId = alternatives[rightIndex].AlternativeId;

                    int leftWins = 0;
                    int rightWins = 0;

                    foreach (var p in people)
                    {
                        int leftResultRank = p.Results
                            .First(r => r.AlternativeId == leftAlternativeId)
                            .Rank;
                        int rightResultRank = p.Results.First(r => r.AlternativeId == rightAlternativeId).Rank;

                        if (rightResultRank < leftResultRank)
                        {
                            rightWins++;
                        }
                        else
                        {
                            leftWins++;
                        }
                    }

                    if (leftWins < rightWins)
                    {
                        pointsDictionary[leftAlternativeId] -= 1;
                        pointsDictionary[rightAlternativeId] += 1;
                    }
                    else if (leftWins > rightWins)
                    {
                        pointsDictionary[leftAlternativeId] += 1;
                        pointsDictionary[rightAlternativeId] -= 1;
                    }
                }
            }

            foreach (var p in people)
            {
                GroupVoteResultModelElement detail = new GroupVoteResultModelElement
                {
                    PersonName = p.Name,
                    SelectedAlternatives = p.Results
                        .OrderBy(r => r.Rank)
                        .Select(r => r.Alternative.Name)
                        .ToList()
                };
                model.Details.Add(detail);
            }

            model.Points = pointsDictionary.Max(el => el.Value);
            model.WinnerName = alternatives
                .First(a => a.AlternativeId == pointsDictionary.First(el => el.Value == model.Points).Key)
                .Name;

            return View(model);
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}