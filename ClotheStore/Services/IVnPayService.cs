using ClotheStore.ModelView;

namespace ClotheStore.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestVM model);
        VnPayResponseVM PaymentExecute(IQueryCollection collection);

    }
}
