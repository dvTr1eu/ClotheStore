using ClotheStore.Extension;
using Microsoft.AspNetCore.Mvc;

namespace ClotheStore.ModelView.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}
