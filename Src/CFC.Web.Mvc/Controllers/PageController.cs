using System.Collections.Generic;
using System.Web;
using CFC.Domain;
using CFC.Services.ContentService;
using CFC.Web.Mvc.Attributes;
using CFC.Web.Mvc.Models;
using CFC.Web.Mvc.Models.Page;
using CFC.Web.Mvc.Resources;
using CFC.Web.Mvc.Wrappers;

namespace CFC.Web.Mvc.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Basic content pages
    /// </summary>
    public class PageController : Controller
    {
        private readonly IContentService _contentService;

        public PageController(IContentService contentService)
        {
            _contentService = contentService;
        }

        /// <summary>
        /// Render a page as a partial block (no surrounding HTML)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Block(string id)
        {
            var page = _contentService.GetPageByName(id);

            if (page == null)
            {
                throw new HttpException(404, "Page not found.");
            }
            var vm = new PageViewModel(page);
            return View("Block", vm);
        }

        [RequiresRole(Role.Role1)]
        public ActionResult List()
        {
            IList<Page> pages = _contentService.GetPages();
            var vm = new PageListViewModel(pages);
            return View("List", vm);
        }

        [RequiresRole(Role.Role1)]
        public ActionResult Edit(int id)
        {
            var page = _contentService.GetPageById(id);

            if (page == null)
            {
                throw new HttpException(404, "Page not found.");
            }
            var vm = new PageViewModel(page);
            return View("Edit", vm);
        }

        [RequiresRole(Role.Role1), HttpPost, ValidateInput(false)]
        public ActionResult Edit(EditPageInputModel inputModel)
        {
            var page = _contentService.GetPageById(inputModel.Id);

            if (page == null)
            {
                throw new HttpException(404, "Page not found.");
            }

            page.Title = inputModel.Title;
            page.Content = inputModel.Content;

            var validationState = new ModelStateWrapper(ModelState);
            var vm = new JsonResponseModel();

            if (_contentService.ValidatePage(page, validationState))
            {
                _contentService.UpdatePage(page);
                vm.Success = true;
                vm.Message = AppResources.PageUpdated;
            }

            vm.Errors = validationState.Errors;
            
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        public new ActionResult View(string id)
        {
            var page = _contentService.GetPageByName(id);

            if (page == null)
            {
                throw new HttpException(404, "Page not found.");
            }
            var vm = new PageViewModel(page);
            return View("View", vm);
        }

        public ActionResult FAQ()
        {
            return base.View("FAQ");
        }

    }
}
