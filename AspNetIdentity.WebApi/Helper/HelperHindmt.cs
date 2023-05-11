using AspNetIdentity.WebApi.Infrastructure;
using System;
using System.Security.Cryptography;

//using NLog;
using System.Text;

namespace AngularJSAuthentication.API
{
    public class CommonHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        //  private static //logger //logger = LogManager.GetCurrentClass//logger();
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        //public static void refreshItemMaster(int warid, int catid)
        //{
        //    try
        //    {
        //        CommonHelper h = new CommonHelper();
        //        ApplicationDbContext context = new ApplicationDbContext();
        //        var cachePolicty = new CacheItemPolicy();
        //        cachePolicty.AbsoluteExpiration = h.indianTime.AddSeconds(1);
        //        ControllerV1.SubCatItemHindmtController.WRSITEM item = new ControllerV1.SubCatItemHindmtController.WRSITEM();
        //        var cache = MemoryCache.Default;
        //        var subsubcategory = context.SubsubCategorys.Where(x => x.IsActive == true && x.Categoryid == catid).Select(x => new ControllerV1.SubCatItemHindmtController.factorySubSubCategory()
        //        {
        //            Categoryid = x.Categoryid,
        //            SubCategoryId = x.SubCategoryId,
        //            SubsubCategoryid = x.SubsubCategoryid,
        //            SubsubcategoryName = x.SubsubcategoryName
        //        }).ToList();

        //        var ItemMasters = context.itemMasters.Where(x => x.active == true && x.Categoryid == catid && x.WarehouseId == warid).Select(x => new ControllerV1.SubCatItemHindmtController.factoryItemdata()
        //        {
        //            WarehouseId = x.WarehouseId,
        //            CompanyId = x.CompanyId,
        //            Categoryid = x.Categoryid,
        //            Discount = x.Discount,
        //            ItemId = x.ItemId,
        //            ItemNumber = x.Number,
        //            itemname = x.SellingUnitName,
        //            LogoUrl = x.SellingSku,
        //            MinOrderQty = x.MinOrderQty,
        //            price = x.price,
        //            SubCategoryId = x.SubCategoryId,
        //            SubsubCategoryid = x.SubsubCategoryid,
        //            TotalTaxPercentage = x.TotalTaxPercentage,
        //            UnitPrice = x.UnitPrice,
        //            VATTax = x.VATTax,
        //            SellingUnitName = x.SellingUnitName,
        //            SellingSku = x.SellingSku,
        //            HindiName = x.HindiName,
        //            marginPoint = x.marginPoint,
        //            promoPerItems = x.promoPerItems

        //        }).ToList();
        //        cache.Remove("CAT" + warid.ToString() + catid.ToString());
        //        item.SubsubCategories = subsubcategory;
        //        item.ItemMasters = ItemMasters;
        //        cache.Add("CAT" + warid.ToString() + catid.ToString(), item, cachePolicty);
        //    }
        //    catch (Exception ex)
        //    {
        //        ////logger.Error("Error in ItemMaster " + ex.Message);
        //        ////logger.Info("End  ItemMaster: ");
        //    }
        //}

        //public static void refreshItemMaster(int warehouseid)
        //{
        //    CommonHelper h = new CommonHelper();
        //    ApplicationDbContext context = new ApplicationDbContext();
        //    customeritems ibjtosend = new customeritems();

        //    var cachePolicty = new CacheItemPolicy();
        //    cachePolicty.AbsoluteExpiration = h.indianTime.AddHours(1);
        //    var cache = MemoryCache.Default;
        //    var dbware = context.DbWarehousesubsubcats.Where(x => x.WarehouseId == warehouseid).ToList();

        //    List<Categories> categories = new List<Categories>();
        //    foreach (var d in dbware)
        //    {
        //        var subsubcategory = context.SubsubCategorys.Where(x => x.IsActive == true && x.SubsubCategoryid == d.SubsubCategoryid).FirstOrDefault();
        //        var cat = context.CategorysHindmt.Where(x => x.IsActive == true && x.Deleted == false && x.Categoryid == subsubcategory.Categoryid).FirstOrDefault();
        //        if (cat != null)
        //        {
        //            categories.Add(new Categories()
        //            {
        //                Categoryid = cat.Categoryid,
        //                CategoryName = cat.CategoryName,
        //                BaseCategoryId = cat.BaseCategoryId,
        //                LogoUrl = cat.LogoUrl
        //            });
        //        }
        //    }
        //    cache.Remove(warehouseid.ToString());
        //    ibjtosend.Basecats = context.BaseCategoryDb.Where(x => x.IsActive == true && x.Deleted == false).Select(s => new Basecats() { BaseCategoryId = s.BaseCategoryId, BaseCategoryName = s.BaseCategoryName, LogoUrl = s.LogoUrl });
        //    ibjtosend.Categories = categories;
        //    ibjtosend.SubCategories = context.SubCategorysHindmt.Where(x => x.IsActive == true && x.Deleted == false).Select(s => new SubCategories() { SubCategoryId = s.SubCategoryId, SubcategoryName = s.SubcategoryName, Categoryid = s.Categoryid });
        //    cache.Add(warehouseid.ToString(), ibjtosend, cachePolicty);
        //}

        //public static customeritems getItemMaster(int warehouseid)
        //{
        //    CommonHelper h = new CommonHelper();
        //    ApplicationDbContext db = new ApplicationDbContext();
        //    customeritems ibjtosend = new customeritems();

        //    var cachePolicty = new CacheItemPolicy();
        //    cachePolicty.AbsoluteExpiration = h.indianTime.AddSeconds(1);

        //    var cache = MemoryCache.Default;
        //    if (cache.Get("CategoryItem".ToString()) == null)
        //    {
        //        cache.Remove("CategoryItem".ToString());
        //        /// Warehouse
        //        ibjtosend.Basecats = db.BaseCategoryDb.Where(x => x.IsActive == true).Select(s => new Basecats() { BaseCategoryId = s.BaseCategoryId, BaseCategoryName = s.BaseCategoryName, LogoUrl = s.LogoUrl });
        //        List<Categories> categories = new List<Categories>();
        //        List<SubCategories> subcategories = new List<SubCategories>();
        //        var cat = db.CategorysHindmt.Where(x => x.IsActive == true).ToList();

        //        var itemmasters = db.itemMasters.Where(x => x.active == true && x.Deleted == false && x.WarehouseId == warehouseid).ToList();
        //        foreach (var kk in cat)
        //        {
        //            foreach (var d in itemmasters)
        //            {
        //                if (kk.Categoryid == d.Categoryid)
        //                {
        //                    if (categories.Count != 0)
        //                    {
        //                        foreach (var dd in categories)
        //                        {
        //                            if (dd.Categoryid == kk.Categoryid)
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    categories.Add(new Categories()
        //                    {
        //                        Categoryid = kk.Categoryid,
        //                        CategoryName = kk.CategoryName,
        //                        BaseCategoryId = kk.BaseCategoryId,
        //                        LogoUrl = kk.LogoUrl
        //                    });
        //                    break;
        //                }
        //            }
        //        }
        //        //foreach (var d in cat)
        //        //{
        //        //    categories.Add(new Categories()
        //        //    {
        //        //        Categoryid = d.Categoryid,
        //        //        CategoryName = d.CategoryName,
        //        //        BaseCategoryId = d.BaseCategoryId,
        //        //        LogoUrl = d.LogoUrl
        //        //    });
        //        //}

        //        ibjtosend.Categories = categories;
        //        var subcate = db.SubCategorysHindmt.Where(x => x.IsActive == true).Select(s => new SubCategories() { SubCategoryId = s.SubCategoryId, SubcategoryName = s.SubcategoryName, Categoryid = s.Categoryid });
        //        foreach (var kk in subcate)
        //        {
        //            foreach (var d in itemmasters)
        //            {
        //                if (kk.SubCategoryId == d.SubCategoryId)
        //                {
        //                    if (subcategories.Count != 0)
        //                    {
        //                        foreach (var dd in subcategories)
        //                        {
        //                            if (dd.SubCategoryId == kk.SubCategoryId)
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    subcategories.Add(new SubCategories()
        //                    {
        //                        SubCategoryId = kk.SubCategoryId,
        //                        SubcategoryName = kk.SubcategoryName,
        //                        Categoryid = kk.Categoryid,
        //                    });
        //                    break;
        //                }
        //            }
        //        }
        //        ibjtosend.SubCategories = subcategories;
        //        cache.Add("CategoryItem".ToString(), ibjtosend, cachePolicty);
        //    }
        //    else
        //    {
        //        ibjtosend = (customeritems)cache.Get("CategoryItem".ToString());
        //    }
        //    return ibjtosend;
        //}

        #region For add Supplier Payment

        //public bool AddSupplierPayment(SupplierPaymentDC paymentDC)
        //{
        //    using (var context = new ApplicationDbContext())
        //    {
        //        var supp = context.Suppliers.Where(x => x.SupplierId == paymentDC.SupplierId).FirstOrDefault();
        //        var Purchase = context.DPurchaseOrderMaster.Where(x => x.PurchaseOrderId == paymentDC.PurchaseOrderId).FirstOrDefault();
        //        var paymentdata = context.SupplierPaymentDataDB.Where(x => x.SupplierId == paymentDC.SupplierId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

        //        SupplierPaymentDataHindmt paydata = new SupplierPaymentDataHindmt();
        //        paydata.PurchaseOrderId = Purchase.PurchaseOrderId;
        //        paydata.InVoiceNumber = Purchase.GrNumber;
        //        paydata.Refrence = paymentDC.RefrenceNumber;
        //        paydata.CreditInVoiceAmount = paymentDC.Amount;
        //        paydata.PaymentStatusCorD = "Credit";
        //        paydata.VoucherType = "Payment";
        //        if (paymentdata != null)
        //        {
        //            paydata.ClosingBalance = paydata.ClosingBalance + paymentDC.Amount;
        //        }
        //        else
        //        {
        //            paydata.ClosingBalance = paymentDC.Amount;
        //        }
        //        paydata.CompanyId = 1;
        //        paydata.WarehouseId = Purchase.WarehouseId ?? 0;
        //        paydata.InVoiceDate = Purchase.CreationDate;
        //        paydata.SupplierId = Purchase.SupplierId;
        //        paydata.SupplierName = Purchase.SupplierName;
        //        paydata.CreatedDate = DateTime.Now;
        //        paydata.UpdatedDate = DateTime.Now;
        //        context.SupplierPaymentDataDB.Add(paydata);
        //        if (context.SaveChanges() > 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        #endregion For add Supplier Payment

        /// <summary>
        /// Version 2
        /// Create by 01/02/2019 for new app version 2
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        //public static customeritems getItemMasterv2(int warehouseid, string lang)
        //{
        //    CommonHelper h = new CommonHelper();
        //    ApplicationDbContext db = new ApplicationDbContext();
        //    customeritems ibjtosend = new customeritems();

        //    var cachePolicty = new CacheItemPolicy();
        //    cachePolicty.AbsoluteExpiration = h.indianTime.AddSeconds(1);

        //    var cache = MemoryCache.Default;
        //    if (cache.Get("Category".ToString()) == null)
        //    {
        //        cache.Remove("Category".ToString());

        //        ibjtosend.Basecats = db.BaseCategoryDb.Where(x => x.IsActive == true).Select(s => new Basecats() { BaseCategoryId = s.BaseCategoryId, BaseCategoryName = s.BaseCategoryName, LogoUrl = s.LogoUrl, HindiName = s.HindiName }).ToList();
        //        foreach (var kk in ibjtosend.Basecats)
        //        {
        //            if (lang == "hi")
        //            {
        //                if (kk.HindiName != null && kk.HindiName != "{nan}" && kk.HindiName != "")
        //                {
        //                    kk.BaseCategoryName = kk.HindiName;
        //                }
        //            }
        //        }
        //        List<Categories> categories = new List<Categories>();
        //        List<SubCategories> subcategories = new List<SubCategories>();
        //        var cat = db.CategorysHindmt.Where(x => x.IsActive == true).ToList();
        //        var itemmasters = db.itemMasters.Where(x => x.active == true && x.Deleted == false && x.WarehouseId == warehouseid).ToList();
        //        foreach (var kk in cat)
        //        {
        //            if (lang == "hi")
        //            {
        //                if (kk.HindiName != null && kk.HindiName != "{nan}" && kk.HindiName != "")
        //                {
        //                    kk.CategoryName = kk.HindiName;
        //                }
        //            }
        //            foreach (var d in itemmasters)
        //            {
        //                if (kk.Categoryid == d.Categoryid)
        //                {
        //                    if (categories.Count != 0)
        //                    {
        //                        foreach (var dd in categories)
        //                        {
        //                            if (dd.Categoryid == kk.Categoryid)
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    categories.Add(new Categories()
        //                    {
        //                        Categoryid = kk.Categoryid,
        //                        CategoryName = kk.CategoryName,
        //                        BaseCategoryId = kk.BaseCategoryId,
        //                        LogoUrl = kk.LogoUrl
        //                    });
        //                    break;
        //                }
        //            }
        //        }
        //        ibjtosend.Categories = categories;
        //        var subcate = db.SubCategorysHindmt.Where(x => x.IsActive == true).Select(s => new SubCategories() { SubCategoryId = s.SubCategoryId, SubcategoryName = s.SubcategoryName, Categoryid = s.Categoryid, HindiName = s.HindiName }).ToList();
        //        foreach (var kk in subcate)
        //        {
        //            if (lang == "hi")
        //            {
        //                if (kk.HindiName != null && kk.HindiName != "{nan}" && kk.HindiName != "")
        //                {
        //                    kk.SubcategoryName = kk.HindiName;
        //                }
        //            }
        //            foreach (var d in itemmasters)
        //            {
        //                if (kk.SubCategoryId == d.SubCategoryId)
        //                {
        //                    if (subcategories.Count != 0)
        //                    {
        //                        foreach (var dd in subcategories)
        //                        {
        //                            if (dd.SubCategoryId == kk.SubCategoryId)
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    subcategories.Add(new SubCategories()
        //                    {
        //                        SubCategoryId = kk.SubCategoryId,
        //                        SubcategoryName = kk.SubcategoryName,
        //                        Categoryid = kk.Categoryid,
        //                    });
        //                    break;
        //                }
        //            }
        //        }
        //        ibjtosend.SubCategories = subcategories;
        //        cache.Add("Category".ToString(), ibjtosend, cachePolicty);
        //    }
        //    else
        //    {
        //        ibjtosend = (customeritems)cache.Get("Category".ToString());
        //    }
        //    return ibjtosend;
        //}

        //public static void refreshCategory()
        //{
        //    ApplicationDbContext db = new ApplicationDbContext();
        //    var dbware = db.WarehousesHind.Where(x => x.Deleted == false).ToList();
        //    foreach (var d in dbware)
        //    {
        //        refreshItemMaster(d.WarehouseId);
        //    }
        //}

        //public static void refreshsubsubCategory(int id)
        //{
        //    ApplicationDbContext db = new ApplicationDbContext();
        //    List<WarehouseHindmt> warehouse = db.WarehousesHind.Where(x => x.Deleted == false).ToList();
        //    foreach (var w in warehouse)
        //    {
        //        refreshItemMaster(w.WarehouseId, id);
        //    }
        //}
    }

    public class EpayAES256
    {
        private static string IV = "94A150D23F2A99BA"; //UAT

        //private static string PASSWORD = "PASSWORD_VALUE";
        //private static string SALT = "SALT_VALUE";
        private static string KEY =
       "9132FDD02ABAABF7D3A0B4BE8B2D5F77"; //UAT

        private static string IVLive = "FC1838E5A0FC5975"; //Live
        private static string KEYLive = "B39735FA6CF7BAE0CB2F49BE7FD0813F"; //Live

        public static string EncryptAndEncode(string raw)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                ICryptoTransform e = GetCryptoTransform(csp, true);
                byte[] inputBuffer = Encoding.UTF8.GetBytes(raw);
                byte[] output = e.TransformFinalBlock(inputBuffer,
               0, inputBuffer.Length);
                string encrypted = Convert.ToBase64String(output);
                return encrypted;
            }
        }

        public static string DecodeAndDecrypt(string encrypted)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                var d = GetCryptoTransform(csp, false);
                byte[] output = Convert.FromBase64String(encrypted);
                byte[] decryptedOutput = d.TransformFinalBlock
               (output, 0, output.Length);
                string decypted = Encoding.UTF8.GetString
               (decryptedOutput);
                return decypted;
            }
        }

        public static string createChecksum(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.
               UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString().ToUpper();
            }
        }

        private static ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            //var spec = new Rfc2898DeriveBytes(Encoding.UTF8.
            //GetBytes(PASSWORD), Encoding.UTF8.GetBytes(SALT), 65536);
            //byte[] key = spec.GetBytes(16);

            csp.IV = Encoding.UTF8.GetBytes(IV);
            csp.Key = Encoding.UTF8.GetBytes(KEY);
            if (encrypting)
            {
                return csp.CreateEncryptor();
            }
            return csp.CreateDecryptor();
        }

        public static string DecodeAndDecryptLive(string encrypted)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                var d = GetCryptoTransformlive(csp, false);
                byte[] output = Convert.FromBase64String(encrypted);
                byte[] decryptedOutput = d.TransformFinalBlock
               (output, 0, output.Length);
                string decypted = Encoding.UTF8.GetString
               (decryptedOutput);
                return decypted;
            }
        }

        private static ICryptoTransform GetCryptoTransformlive(AesCryptoServiceProvider csp, bool encrypting)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            //var spec = new Rfc2898DeriveBytes(Encoding.UTF8.
            //GetBytes(PASSWORD), Encoding.UTF8.GetBytes(SALT), 65536);
            //byte[] key = spec.GetBytes(16);

            csp.IV = Encoding.UTF8.GetBytes(IVLive);
            csp.Key = Encoding.UTF8.GetBytes(KEYLive);
            if (encrypting)
            {
                return csp.CreateEncryptor();
            }
            return csp.CreateDecryptor();
        }
    }
}