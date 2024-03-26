using System.Text.RegularExpressions;

namespace ProjectA.Extension
{
    public static class Extension
    {
        public static string ToVnd(this double donGia)
        {
            return donGia.ToString("#,##0") + " đ";
        }
    }
}
