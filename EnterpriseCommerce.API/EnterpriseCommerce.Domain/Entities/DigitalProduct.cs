using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Entities
{
    public class DigitalProduct:Product
    {
        // رابط تحميل الملف الرقمي أو الكورس
        public string DownloadUrl { get; set; } = string.Empty;

        // كود التفعيل أو السيريال نمبر اللي هيتبعت للزبون بعد الشراء
        public string ActivationCode { get; set; } = string.Empty;
    }
}
