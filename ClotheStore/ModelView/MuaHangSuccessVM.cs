using System.ComponentModel.DataAnnotations;

namespace ClotheStore.ModelView
{
    public class MuaHangSuccessVM
    {
        [Key]
        public int DonHangID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
