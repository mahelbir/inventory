using System;

namespace Inventory.ViewModels
{
    public class PageableListViewModel
    {

        public int PageSize { get; set; } = 10; // Her sayfada kaç kayıt gösterileceği

        public int MaxPagesToShow { get; set; } = 5; // Aynı anda gösterilecek maksimum sayfa düğmesi sayısı

        public int TotalRows { get; set; } // Toplam kayıt sayısı

        public int TotalPages // Toplam sayfa sayısı
        {
            get
            {
                return (int)Math.Ceiling((double)TotalRows / PageSize);
            }
        }

        // Hangi sayfada olduğumuzu belirler
        private int pageIndex = 1;
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = Math.Max(1, value); } // Sayfa numarası 1'den küçük olamaz
        }

        public int StartPage // Başlangıç sayfası
        {
            get
            {
                int startPage = PageIndex - (MaxPagesToShow / 2);
                return Math.Max(1, startPage);
            }
        }

        public int EndPage // Bitiş sayfası
        {
            get
            {
                int endPage = StartPage + MaxPagesToShow - 1;
                return Math.Min(TotalPages, endPage);
            }
        }

        public bool HasPreviousPage  // Önceki sayfa var mı
        {
            get { return PageIndex > 1; }
        }

        public bool HasNextPage // Sonraki sayfa var mı
        {
            get { return PageIndex < TotalPages; }
        }

    }
}
