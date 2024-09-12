using ClotheStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClotheStore.Controllers
{
    public class LocationController : Controller
    {
        private readonly ClothesStoreContext _context;
        public LocationController(ClothesStoreContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region ======================= GetLocaion =======================

        public ActionResult QuanHuyenList(int locationId)
        {
            var QuanHuyens = _context.Locations.OrderBy(x => x.LocationId)
                .Where(x => x.ParentCode == locationId && x.Levels == 2)
                .OrderBy(x => x.Name)
                .ToList();
            return Json(QuanHuyens);
        }

        public ActionResult PhuongXaList(int locationId)
        {
            var PhuongXas = _context.Locations.OrderBy(x => x.LocationId)
                .Where(x => x.ParentCode == locationId && x.Levels == 3)
                .OrderBy(x => x.Name)
                .ToList();
            return Json(PhuongXas);
        }

        #endregion
    }
}
