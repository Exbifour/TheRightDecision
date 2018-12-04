using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheRightDecision.Data;
using TheRightDecision.Models;
using TheRightDecision.Models.ViewModels;

namespace TheRightDecision.Controllers
{
    public class SetupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SetupController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Alternatives
        [HttpGet]
        public IActionResult Alternatives()
        {
            IEnumerable<Alternative> alternatives = _context.Alternatives;
            return View(alternatives);
        }

        public IActionResult CreateAlternative()
        {
            Alternative newAlternative = new Alternative();
            return View("EditAlternative", newAlternative);
        }

        public IActionResult EditAlternative(int id)
        {
            Alternative alternative = _context.Alternatives.First(a => a.AlternativeId == id);
            return View(alternative);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditAlternative(Alternative alternative)
        {
            if (ModelState.IsValid)
            {
                if (alternative.AlternativeId < 0)
                {
                    await _context.Alternatives.AddAsync(alternative);
                }
                else
                {
                    _context.Alternatives.Update(alternative);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Alternatives");
            }

            return View("EditAlternative", alternative);
        }

        public async Task<IActionResult> RemoveAlternative(int id)
        {
            Alternative alternative = _context.Alternatives.First(a => a.AlternativeId == id);
            _context.Alternatives.Remove(alternative);
            await _context.SaveChangesAsync();

            return RedirectToAction("Alternatives");
        }
        #endregion

        #region Criteria
        public IActionResult Criteria()
        {
            IEnumerable<Criterion> criteria = _context.Criteria;
            return View(criteria);
        }

        public IActionResult CreateCriterion()
        {
            Criterion newCriterion = new Criterion();
            return View("EditCriterion", newCriterion);
        }

        public IActionResult EditCriterion(int id)
        {
            Criterion criterion = _context.Criteria.First(c => c.CriterionId == id);
            return View(criterion);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditCriterion(Criterion criterion)
        {
            if (ModelState.IsValid)
            {
                if (criterion.CriterionId < 0)
                {
                    await _context.Criteria.AddAsync(criterion);
                }
                else
                {
                    _context.Criteria.Update(criterion);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Criteria");
            }

            return View("EditCriterion", criterion);
        }

        public async Task<IActionResult> RemoveCriterion(int id)
        {
            Criterion criterion = _context.Criteria.First(c => c.CriterionId == id);
            _context.Criteria.Remove(criterion);
            await _context.SaveChangesAsync();

            return RedirectToAction("Criteria");
        }
        #endregion

        #region Marks
        public IActionResult Marks()
        {
            IEnumerable<Criterion> criteria = _context.Criteria.Include(c => c.Marks);
            return View(criteria);
        }

        public IActionResult CreateMark(int criterionId)
        {
            Mark newMark = new Mark()
            {
                CriterionId = criterionId,
                Criterion = _context.Criteria.First(c => c.CriterionId == criterionId)
            };
            return View("EditMark", newMark);
        }

        public IActionResult EditMark(int id)
        {
            Mark mark = _context.Marks
                .Include(m => m.Criterion)
                .First(m => m.MarkId == id);
            return View(mark);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditMark(Mark mark, string criterionType)
        {
            if (ModelState.IsValid)
            {
                if (criterionType == "Quantitative")
                {
                    mark.Number = double.Parse(mark.Name, System.Globalization.CultureInfo.InvariantCulture);
                }

                if (mark.MarkId < 1)
                {
                    NormalizeNewOrUpdatedMark(mark);
                    _context.Marks.Add(mark);
                }
                else
                {
                    NormalizeNewOrUpdatedMark(mark);
                    _context.Marks.Update(mark);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Marks");
            }

            ViewBag.Criteria = _context.Criteria
                .Select(c => new SelectListItem() { Value = c.CriterionId.ToString(), Text = c.Name })
                .ToList();
            return View("EditMarks", mark);
        }

        private void NormalizeNewOrUpdatedMark(Mark mark)
        {
            double etalon;
            string criterionOprimalityType = _context.Criteria.First(c => c.CriterionId == mark.CriterionId).OptimalityType;
            bool optimalityTypeIsMax = criterionOprimalityType == "Max";

            if (optimalityTypeIsMax)
            {
                etalon = _context.Marks
                    .Where(m => m.CriterionId == mark.CriterionId)
                    .Max(m => m.Number);

                if (mark.Number > etalon)
                {
                    NormalizeMarksByCriteria(mark.CriterionId, mark.Number, optimalityTypeIsMax);
                    mark.Normalized = 1;
                }
                else
                {
                    NormalizeOneMark(mark, etalon, optimalityTypeIsMax);
                }
            }
            else
            {
                etalon = _context.Marks
                    .Where(m => m.CriterionId == mark.CriterionId)
                    .Min(m => m.Number);

                if (mark.Number < etalon)
                {
                    NormalizeMarksByCriteria(mark.CriterionId, mark.Number, optimalityTypeIsMax);
                    mark.Normalized = 1;
                }
                else
                {
                    NormalizeOneMark(mark, etalon, optimalityTypeIsMax);
                }
            }
        }

        private void NormalizeOneMark(Mark mark, double etalon, bool optimalityTypeIsMax)
        {
            if (optimalityTypeIsMax)
            {
                mark.Normalized = Math.Round(mark.Number / etalon, 2);
            }
            else
            {
                mark.Normalized = Math.Round(etalon / mark.Number, 2);
            }
            _context.Marks.Update(mark);
        }

        private void NormalizeMarksByCriteria(int criterionId)
        {
            string criterionOprimalityType = _context.Criteria.First(c => c.CriterionId == criterionId).OptimalityType;
            bool optimalityTypeIsMax = criterionOprimalityType == "Max";

            double newEtalon;
            if(optimalityTypeIsMax)
            {
                newEtalon = _context.Marks
                    .Where(m => m.CriterionId == criterionId)
                    .Max(m => m.Number);
            }
            else
            {
                newEtalon = _context.Marks
                    .Where(m => m.CriterionId == criterionId)
                    .Min(m => m.Number);
            }

            NormalizeMarksByCriteria(criterionId, newEtalon, optimalityTypeIsMax);
        }

        private void NormalizeMarksByCriteria(int criterionId, double etalon, bool optimalityTypeIsMax)
        {
            IQueryable<Mark> marks = _context.Marks.Where(m => m.CriterionId == criterionId);

            foreach(Mark mark in marks)
            {
                NormalizeOneMark(mark, etalon, optimalityTypeIsMax);
            }
            _context.Marks.UpdateRange(marks);
        }

        private bool MarkIsEtalon(Mark mark)
        {
            string criterionOprimalityType = _context.Criteria.First(c => c.CriterionId == mark.CriterionId).OptimalityType;
            bool optimalityTypeIsMax = criterionOprimalityType == "Max";

            double etalon;
            bool markIsEtalon = false;

            if(optimalityTypeIsMax)
            {
                etalon = _context.Marks
                    .Where(m => m.CriterionId == mark.CriterionId)
                    .Max(m => m.Number);
                if(mark.Number == etalon)
                {
                    bool onlyOneEtalon = IsThereOnlyOneEtalon(mark.CriterionId, etalon);
                    if(onlyOneEtalon)
                    {
                        markIsEtalon = true;
                    }
                }
                else if(mark.Number > etalon)
                {
                    markIsEtalon = true;
                }
            }
            else
            {
                etalon = _context.Marks
                       .Where(m => m.CriterionId == mark.CriterionId)
                       .Min(m => m.Number);
                if (mark.Number == etalon)
                {
                    bool onlyOneEtalon = IsThereOnlyOneEtalon(mark.CriterionId, etalon);
                    if (onlyOneEtalon)
                    {
                        markIsEtalon = true;
                    }
                }
                else if (mark.Number < etalon)
                {
                    markIsEtalon = true;
                }
            }

            return markIsEtalon;
        }

        private bool IsThereOnlyOneEtalon(int criterionId, double etalon)
        {
            int numberOfEtalons = _context.Marks
                .Where(m => m.CriterionId == criterionId && m.Number == etalon)
                .Count();
            return numberOfEtalons == 1;
        }

        public async Task<IActionResult> RemoveMark(int id)
        {
            Mark mark = _context.Marks.First(m => m.MarkId == id);

            bool markIsEtalon = MarkIsEtalon(mark);

            _context.Marks.Remove(mark);
            await _context.SaveChangesAsync();

            if (markIsEtalon)
            {
                NormalizeMarksByCriteria(mark.CriterionId);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Marks");
        }

        #endregion

        #region Vectors
        public IActionResult Vectors()
        {
            IEnumerable<Alternative> alternatives = _context.Alternatives
                .Include(a => a.Vectors)
                    .ThenInclude(v => v.Mark);
            ViewBag.Criteria = _context.Criteria;
            return View(alternatives);
        }

        public IActionResult CreateVector(int alternativeId, int criterionId)
        {
            Vector vector = new Vector
            {
                Alternative = _context.Alternatives.First(a => a.AlternativeId == alternativeId)
            };
            ViewBag.Criterion = _context.Criteria.First(c => c.CriterionId == criterionId);
            ViewBag.Marks = _context.Marks
                .Where(m => m.CriterionId == criterionId)
                .OrderByDescending(m => m.Normalized)
                .Select(m => new SelectListItem(m.Name, m.MarkId.ToString()));
            return View("EditVector", vector);
        }

        public IActionResult EditVector(int id)
        {
            Vector vector = _context.Vectors
                .Include(v => v.Mark)
                    .ThenInclude(m => m.Criterion)
                .Include(v => v.Alternative)
                .First(v => v.VectorId == id);
            Criterion criterion = vector.Mark.Criterion;
            ViewBag.Criterion = criterion;
            ViewBag.Marks = _context.Marks
                .Where(m => m.CriterionId == criterion.CriterionId)
                .Select(m => new SelectListItem(m.Name, m.MarkId.ToString()));
            return View("EditVector", vector);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditVector(Vector vector)
        {
            if (vector.VectorId < 0)
            {
                await _context.Vectors.AddAsync(vector);
            }
            else
            {
                _context.Vectors.Update(vector);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Vectors");
        }

        public async Task<IActionResult> RemoveVector(int id)
        {
            Vector vector = _context.Vectors.First(v => v.VectorId == id);
            _context.Vectors.Remove(vector);
            await _context.SaveChangesAsync();

            return RedirectToAction("Vectors");
        }
        #endregion
    }
}