using System.IO;
using System.Text;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Common;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.Core.Json;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ReopenMemoController : ApiController
    {
        [HttpGet]
        [Route("api/ReopenMemo/GetReopenMemo/{projectId}")]
        public IHttpActionResult GetReopenMemo(string projectId)
        {
            var memo = ReopenMemo.GetReopenMemo(projectId);
            if (string.IsNullOrEmpty(memo.ExteriorAfterImg1))
                memo.ExteriorAfterImgURL1 = "../Images/mcd_logo.png";
            else
                memo.ExteriorAfterImgURL1 = SiteFilePath.UploadFiles_URL + "/" + memo.ExteriorAfterImg1;
            if (string.IsNullOrEmpty(memo.ExteriorAfterImg2))
                memo.ExteriorAfterImgURL2 = "../Images/mcd_logo.png";
            else
                memo.ExteriorAfterImgURL2 = SiteFilePath.UploadFiles_URL + "/" + memo.ExteriorAfterImg2;
            if (string.IsNullOrEmpty(memo.InteriorAfterImg1))
                memo.InteriorAfterImgURL1 = "../Images/mcd_logo.png";
            else
                memo.InteriorAfterImgURL1 = SiteFilePath.UploadFiles_URL + "/" + memo.InteriorAfterImg1;
            if (string.IsNullOrEmpty(memo.InteriorAfterImg2))
                memo.InteriorAfterImgURL2 = "../Images/mcd_logo.png";
            else
                memo.InteriorAfterImgURL2 = SiteFilePath.UploadFiles_URL + "/" + memo.InteriorAfterImg2;
            return Ok(memo);
        }

        [HttpPost]
        [Route("api/ReopenMemo/SaveReopenMemo")]
        public IHttpActionResult SaveReopenMemo(ReopenMemo memo)
        {
            ReopenMemo.SaveReopenMemo(memo);
            return Ok();
        }

        [HttpPost]
        [Route("api/ReopenMemo/UploadImg/{typeCode}/{projectId}")]
        public IHttpActionResult UploadImg(string typeCode, string projectId)
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                HttpFileCollection fileCollect = request.Files;
                for (var i = 0; i < fileCollect.Count; i++)
                {
                    HttpPostedFile file = fileCollect[i];
                    string fileExtension = Path.GetExtension(file.FileName);
                    string internalName = Guid.NewGuid() + fileExtension;
                    string absolutePath = HttpContext.Current.Server.MapPath("~/") + "UploadFiles/" + internalName;
                    file.SaveAs(absolutePath);
                    ReopenMemo memo = ReopenMemo.GetReopenMemo(projectId);
                    if (memo == null)
                        memo = new ReopenMemo();
                    memo.ProjectId = projectId;
                    if (typeCode == "ExteriorAfterImg1")
                        memo.ExteriorAfterImg1 = internalName;
                    else if (typeCode == "ExteriorAfterImg2")
                        memo.ExteriorAfterImg2 = internalName;
                    else if (typeCode == "InteriorAfterImg1")
                        memo.InteriorAfterImg1 = internalName;
                    else if (typeCode == "InteriorAfterImg2")
                        memo.InteriorAfterImg2 = internalName;
                    ReopenMemo.SaveReopenMemo(memo);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/ReopenMemo/DeleteImg/{typeCode}/{projectId}")]
        public IHttpActionResult DeleteImg(string typeCode, string projectId)
        {
            var memo = ReopenMemo.GetReopenMemo(projectId);
            if (typeCode == "ExteriorAfterImg1")
                memo.ExteriorAfterImg1 = "";
            else if (typeCode == "ExteriorAfterImg2")
                memo.ExteriorAfterImg2 = "";
            else if (typeCode == "InteriorAfterImg1")
                memo.InteriorAfterImg1 = "";
            else if (typeCode == "InteriorAfterImg2")
                memo.InteriorAfterImg2 = "";
            ReopenMemo.SaveReopenMemo(memo);
            return Ok();
        }

        [Route("api/ReopenMemo/SendReopenMemo")]
        [HttpPost]
        public IHttpActionResult SendReopenMemo(PostMemo<ReopenMemo> postData)
        {
            var actor = ProjectUsers.GetProjectUser(postData.Entity.ProjectId, ProjectUserRoleCode.AssetActor);
            using (TransactionScope tranScope = new TransactionScope())
            {
                Dictionary<string, string> pdfData = new Dictionary<string, string>();
                if (postData.Entity.ProjectId.ToLower().IndexOf("rebuild") >= 0)
                {
                    pdfData.Add("WorkflowName", "Rebuild");
                }
                else if (postData.Entity.ProjectId.ToLower().IndexOf("majorlease") != -1)
                {
                    pdfData.Add("WorkflowName", "MajorLease");
                }
                else if (postData.Entity.ProjectId.ToLower().IndexOf("renewal") != -1)
                {
                    pdfData.Add("WorkflowName", "Renewal");
                }
                else if (postData.Entity.ProjectId.ToLower().IndexOf("reimage") >= 0)
                {
                    pdfData.Add("WorkflowName", "Reimage");
                }
                else
                {
                    pdfData.Add("WorkflowName", postData.Entity.ProjectId);
                }
                pdfData.Add("ProjectID", postData.Entity.ProjectId);
                pdfData.Add("RegionNameENUS", postData.Entity.Store.StoreBasicInfo.RegionENUS);
                pdfData.Add("RegionNameZHCN", postData.Entity.Store.StoreBasicInfo.RegionZHCN);
                pdfData.Add("MarketNameENUS", postData.Entity.Store.StoreBasicInfo.MarketENUS);
                pdfData.Add("MarketNameZHCN", postData.Entity.Store.StoreBasicInfo.MarketZHCN);
                pdfData.Add("ProvinceNameENUS", postData.Entity.Store.StoreBasicInfo.ProvinceENUS);
                pdfData.Add("ProvinceNameZHCN", postData.Entity.Store.StoreBasicInfo.ProvinceZHCN);
                pdfData.Add("CityNameENUS", postData.Entity.Store.StoreBasicInfo.CityENUS);
                pdfData.Add("CityNameZHCN", postData.Entity.Store.StoreBasicInfo.CityZHCN);
                pdfData.Add("StoreNameENUS", postData.Entity.Store.StoreBasicInfo.NameENUS);
                pdfData.Add("StoreNameZHCN", postData.Entity.Store.StoreBasicInfo.NameZHCN);
                pdfData.Add("USCode", postData.Entity.Store.StoreBasicInfo.StoreCode);

                pdfData.Add("IsMcCafe", postData.Entity.NewMcCafe ? "Y" : "N");
                pdfData.Add("IsKiosk", postData.Entity.NewKiosk ? "Y" : "N");
                pdfData.Add("IsMDS", postData.Entity.NewMDS ? "Y" : "N");
                pdfData.Add("Is24Hour", postData.Entity.Is24H ? "Y" : "N");

                pdfData.Add("TTMNetSales", DataConverter.ToMoney(postData.Entity.TTMNetSales) ?? "&nbsp;");
                var TTMNetSalesYearMonth = "";
                if (postData.Entity.YearMonthList != null && postData.Entity.YearMonthList.Count > 0)
                {
                    foreach (var val in postData.Entity.YearMonthList)
                    {
                        if (val.Value == postData.Entity.TTMNetSales)
                        {
                            TTMNetSalesYearMonth = val.Name;
                            break;
                        }
                    }
                }
                pdfData.Add("TTMNetSalesYearMonth", TTMNetSalesYearMonth ?? "&nbsp;");
                pdfData.Add("IncrementalSales", postData.Entity.IncrementalSales.HasValue ? DataConverter.ToMoney((postData.Entity.IncrementalSales.Value * 100)) : "&nbsp;");

                pdfData.Add("GBDate", postData.Entity.GBDate.HasValue ? postData.Entity.GBDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
                pdfData.Add("ConstCompletionDate", postData.Entity.CompletionDate.HasValue ? postData.Entity.CompletionDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");

                //if (postData.Entity.ProjectId.ToLower().IndexOf("reimage") >= 0)
                //{
                //    pdfData.Add("ReopenDate", postData.Entity.RmgInfo.ReopenDate.HasValue ? postData.Entity.RmgInfo.ReopenDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
                //}
                //else
                //{
                //    pdfData.Add("ReopenDate", postData.Entity.ReopenDate.HasValue ? postData.Entity.RbdInfo.ReopenDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
                //}
                pdfData.Add("ReopenDate", postData.Entity.ReopenDate.HasValue ? postData.Entity.ReopenDate.Value.ToString("yyyy-MM-dd") : "&nbsp;");
                pdfData.Add("DesignConcept", GetConcetpetDesc(postData.Entity.DesignConcept) ?? "&nbsp;");
                pdfData.Add("TotalWriteOff", DataConverter.ToMoney(postData.Entity.WriteOff.TotalWriteOff) ?? "&nbsp;");
                pdfData.Add("TotalReinvestmentBudget", DataConverter.ToMoney(postData.Entity.TotalReinvestmentBudget) ?? "&nbsp;");
                pdfData.Add("OriginalOperationSize", postData.Entity.OriginalOperationSize ?? "&nbsp;");
                pdfData.Add("NewOperationSize", postData.Entity.AftOperationSize ?? "&nbsp;");
                pdfData.Add("OriginalSeatNumber", postData.Entity.OriginalSeatNumber ?? "&nbsp;");
                pdfData.Add("ARSN", postData.Entity.AftARSN ?? "&nbsp;");
                pdfData.Add("AftARPT", postData.Entity.AftARPT ?? "&nbsp;");
                pdfData.Add("PriceTiter", postData.Entity.PriceTiter ?? "&nbsp;");
                if (postData.Entity.ExteriorAfterImgURL1 != "../Images/mcd_logo.png")
                {
                    pdfData.Add("ExteriorAfterImgURL1", HtmlConversionUtility.ImageToBase64(postData.Entity.ExteriorAfterImgURL1, System.Drawing.Imaging.ImageFormat.Png));
                }
                else
                {
                    pdfData.Add("ExteriorAfterImgURL1", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADYAAAAzCAMAAADrVgtcAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6REFDNzNCMkEwQkYyMTFFNEI5MzU4NkQ4NkQxNUM0MTYiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6REFDNzNCMkIwQkYyMTFFNEI5MzU4NkQ4NkQxNUM0MTYiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDpEQUM3M0IyODBCRjIxMUU0QjkzNTg2RDg2RDE1QzQxNiIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDpEQUM3M0IyOTBCRjIxMUU0QjkzNTg2RDg2RDE1QzQxNiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pnx0qhIAAAMAUExURbQADfK1A/rF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");
                }
                if (postData.Entity.ExteriorAfterImgURL2 != "../Images/mcd_logo.png")
                {
                    pdfData.Add("ExteriorAfterImgURL2", HtmlConversionUtility.ImageToBase64(postData.Entity.ExteriorAfterImgURL2, System.Drawing.Imaging.ImageFormat.Png));
                }
                else
                {
                    pdfData.Add("ExteriorAfterImgURL2", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADYAAAAzCAMAAADrVgtcAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6REFDNzNCMkEwQkYyMTFFNEI5MzU4NkQ4NkQxNUM0MTYiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6REFDNzNCMkIwQkYyMTFFNEI5MzU4NkQ4NkQxNUM0MTYiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDpEQUM3M0IyODBCRjIxMUU0QjkzNTg2RDg2RDE1QzQxNiIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDpEQUM3M0IyOTBCRjIxMUU0QjkzNTg2RDg2RDE1QzQxNiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pnx0qhIAAAMAUExURbQADfK1A/rFAsJERN1tB/Xh4d6ZmeKiosxSUrwADqwBAbQdHcITDbQXB7olB+J6BsAMDP/5AOWsrL0FDf/dAMl4eO/NzeaVA60BCdR8fMUbDMorC//nAOqUBfHV1fnt7b0xB/jIAf7VAMsxCv/0AL4JDc5qatmNjdWAgP/oAOeZA60YGLIzM//qAMVRUbssLOm6ursADvrMAemLBf/sAPDOzv/gAMtcXPW9AsEODd11BtFycuuyAq4GBvbBAuq9vfveAeWrA+vDw6oFBf3RAdNICfrp6f/aANySkvbl5c49CcNLS8AKDdBubttjB/Ta2rwLC7xGRt2AA9VSCf/wAMxiYsklC8FLTPHY2LoLC6oACNt+BbUKCuaKBcNMTf/yAO2iBO6eA8INDdNqBcVNTdmGhrwxMcNQUK4GCLsOD7IWFuKNAqoAAKYAAPrx8cxSBaYACeaGBeCEA7Q2NrtJSagACfKvA/z19f/uANFCCagAALgKCrgkJPTd3eaPBd+kpMFMTL8NDM5cBf/8AP/lAMQ+BrsADP3YAawICLkwMK0EBPbFAtdVCN2WlrgADb5KS/jn5++tA7sFDMA9Pb41NaoICK0ECKsCCLAGBrMJCcIPDa4EBMdNBrsDDfnVAc1lZfa6AqsGBq4ECKEAALQ0NL8HDbEKCK8KCrgAD7EICLwMDK4ICK8JCMNPT68JCbAJCcJOTrAJCP//AMRLTK4JCP///7EJCbEJCOCdnaoDA6kDA//YAK8ICcJJSv/hAP7v76cJCcgoC/34+KsPD7MLCq4OCK8ODqcGBp4ACdyqqvv9/b85OfnTAcRLS89ubrALC9OPj/jKAq8KCMFubuCIBO+tBOybBPO4A/G9AtFOCc9ACchPBtV0Bc44Cu3IyNtgB78ODMYkC7AMDLgnJ7YsLL0MC60uLuugBOunAu+nA+2lBMhWVslpaduRkfzy8stgYOODBeWCBbEwMOuoAuypA+6pBO/o6PHR0fCkA7AICL0MDMANDb4MDL8MDMENDa8ICA0STKgAAAWDSURBVHjavNN9XBNlHADwKZt0B+rGfJlzAsqLDB0fDwcy6QARpZehM0FjouRUnHhLzKY4EpmvOHUq2KarqYhD3SbThYpaK7KyN/tkmaZlmlZWpvZe22277m7cXhj4yf7od/88d8997/d7nvs9tDmSUgkZZhcebjw8RDCZKIp6gwIlgpzCX5lD8zNXMAtTJGN2KpzV+VKZzcG50B4RoVyuOjwbqcJSoT2mIlkdtS53YF1oFxaqSDazay5Pz7k8PkSxMOV9oDITzGUO3UMUFZvEFMGHpGIG5zK7ZtKepFBnKhEKsA1swCvyekVMgB1tBILWRSKzRIIzd0iFIhTquL1n2AwIZ2Jo6I97arkQ6umeuYMqhIZ+WL65eHiH0cAEruUlXmD14hpvBLaeVD4WvBtGcS97/0W/2JcnsdnRm+30RWPtwzYkqT3BKMAoxYS2sIZH07jpa68fAsbbs9y0GXnFV7WoX/mZJ7g3TOLla/tDYmApK8sUnc6iHzIaL9lvGxvUgXXhUbONYB5/HxqAlTzBSsgATBLwr05PHJGg9WivJzoStOpAhaVlJ5zP+xnZGzLoZ9ZmLmBgd6QXL51bvC/aqNYmbGTRtUmdFZbW7HZilgAjSzSwDZdZPySZDCbDZ9W9C/jDjOwbkCGH9YyxgdhuV2mZDSOCZD5FMighr3w2JPaKgdriEYU8ulZ8g82em3jha8jtltTsttRb/Izpb14RemgKnzcFZ0yILhhxxrGFplYnQf2FmVe0eHEUohh1vkRJUG35gEkQ3ovQ9HUDCguewLdCrZ1Szadz5H7jZ9TxErFNf5RvvAaJUBF0zZFZOLxDq/agtCsO3mKdDsO6MP/5EgHcnOr0oSTryFs3IIsLqCVl0vUF/B1vd2WveP3nS2RM2CjYxwVEeGdyL/AyL5tob1nalPHp/N59OdZwRp0uaOU6/vJoI8Gic/iZv3P0bRartG+OIL2fPpxRCm9IHu9vlI0yvRu+3SF0DJbqrBjOPhcUrJce65GJITqPl2oSyzylHOtvQkcqyTi6R4VnllSFMX+IgfF83iVTQ83uemnb90LHRancxwSFa8KYzM9MwFghbzZHjv9XvbS22jFeCfsYP3NeV/YaxWQyE1Ar5D1VRUzolan2tReVRDadfjCPP+tdKRbYFOc02iOyTiRTN0gXFwueUxITOv28CRPW6PGRVSdNFQq+jF+i2x9gR3xMJkPx7tZXEazKN6XnSAlFZEsV8ubNyvpEGsrwcOPdbbHIdfFfvBzP8U3pdDo5OZDvx5/26xcvl4cypqTM19xWTFlVFfKDyHRW4qleGbw2nNWcoE7Eq60tJ0+SqpV6gRy0tJxsOdaCTwexo3h1vjdABAFbO0e+aRuMaPBbBLEgGkQDInAQc3Z+F2wsyd6kUKkUKiRiL6xQ4B9RqYoqFApVyfybRVFRTRF7D2rCmSo55t7WbyIqIiqaR8bWn89+Z1Nl5bTcog/OR+Xn3owpik1p/ysKsXVlNtWQr+62b4+7F8eIys5Nmfx67qAF7WNi8p+Ny4i8U5kfc3zQzhdgOCybTXF36hvJk+9Pvc+YX5KxYuB3x2MZGePGPJ6fvGJb8y1GCSOlWQW3dsP6rGrO3t44qHFV2kexq1b/9Gby8adHZcTl3onb1Hxr+zjG6nMaLHxLbMjex1RpkRUjd/bZmTayMZ+Rrekz5PTW9gURkbdUUZEVKWNGgd0wbNfE07vgF+FTmia4rR45O0qlOX0KrG9qAifCGFwPTjyLYd2x1pdAGwySlwbEEERjAzVOECTuD8IgpkFC2BEn1l04wx47Q9jC7tkDw7nwf2Vp/4lhR2l1hx9e2Y7SBp5zOp2H/304nbZdle/R/vz01wMHRj9ELBu97OP3/xFgAEAgj7rRci3dAAAAAElFTkSuQmCC");
                }
                if (postData.Entity.InteriorAfterImgURL1 != "../Images/mcd_logo.png")
                {

                    pdfData.Add("InteriorAfterImgURL1", HtmlConversionUtility.ImageToBase64(postData.Entity.InteriorAfterImgURL1, System.Drawing.Imaging.ImageFormat.Png));
                }
                else
                {
                    pdfData.Add("InteriorAfterImgURL1", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADYAAAAzCAMAAADrVgtcAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6REFDNzNCMkEwQkYyMTFFNEI5MzU4NkQ4NkQxNUM0MTYiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6REFDNzNCMkIwQkYyMTFFNEI5MzU4NkQ4NkQxNUM0MTYiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDpEQUM3M0IyODBCRjIxMUU0QjkzNTg2RDg2RDE1QzQxNiIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDpEQUM3M0IyOTBCRjIxMUU0QjkzNTg2RDg2RDE1QzQxNiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pnx0qhIAAAMAUExURbQADfK1A/rFAsJERN1tB/Xh4d6ZmeKiosxSUrwADqwBAbQdHcITDbQXB7olB+J6BsAMDP/5AOWsrL0FDf/dAMl4eO/NzeaVA60BCdR8fMUbDMorC//nAOqUBfHV1fnt7b0xB/jIAf7VAMsxCv/0AL4JDc5qatmNjdWAgP/oAOeZA60YGLIzM//qAMVRUbssLOm6ursADvrMAemLBf/sAPDOzv/gAMtcXPW9AsEODd11BtFycuuyAq4GBvbBAuq9vfveAeWrA+vDw6oFBf3RAdNICfrp6f/aANySkvbl5c49CcNLS8AKDdBubttjB/Ta2rwLC7xGRt2AA9VSCf/wAMxiYsklC8FLTPHY2LoLC6oACNt+BbUKCuaKBcNMTf/yAO2iBO6eA8INDdNqBcVNTdmGhrwxMcNQUK4GCLsOD7IWFuKNAqoAAKYAAPrx8cxSBaYACeaGBeCEA7Q2NrtJSagACfKvA/z19f/uANFCCagAALgKCrgkJPTd3eaPBd+kpMFMTL8NDM5cBf/8AP/lAMQ+BrsADP3YAawICLkwMK0EBPbFAtdVCN2WlrgADb5KS/jn5++tA7sFDMA9Pb41NaoICK0ECKsCCLAGBrMJCcIPDa4EBMdNBrsDDfnVAc1lZfa6AqsGBq4ECKEAALQ0NL8HDbEKCK8KCrgAD7EICLwMDK4ICK8JCMNPT68JCbAJCcJOTrAJCP//AMRLTK4JCP///7EJCbEJCOCdnaoDA6kDA//YAK8ICcJJSv/hAP7v76cJCcgoC/34+KsPD7MLCq4OCK8ODqcGBp4ACdyqqvv9/b85OfnTAcRLS89ubrALC9OPj/jKAq8KCMFubuCIBO+tBOybBPO4A/G9AtFOCc9ACchPBtV0Bc44Cu3IyNtgB78ODMYkC7AMDLgnJ7YsLL0MC60uLuugBOunAu+nA+2lBMhWVslpaduRkfzy8stgYOODBeWCBbEwMOuoAuypA+6pBO/o6PHR0fCkA7AICL0MDMANDb4MDL8MDMENDa8ICA0STKgAAAWDSURBVHjavNN9XBNlHADwKZt0B+rGfJlzAsqLDB0fDwcy6QARpZehM0FjouRUnHhLzKY4EpmvOHUq2KarqYhD3SbThYpaK7KyN/tkmaZlmlZWpvZe22277m7cXhj4yf7od/88d8997/d7nvs9tDmSUgkZZhcebjw8RDCZKIp6gwIlgpzCX5lD8zNXMAtTJGN2KpzV+VKZzcG50B4RoVyuOjwbqcJSoT2mIlkdtS53YF1oFxaqSDazay5Pz7k8PkSxMOV9oDITzGUO3UMUFZvEFMGHpGIG5zK7ZtKepFBnKhEKsA1swCvyekVMgB1tBILWRSKzRIIzd0iFIhTquL1n2AwIZ2Jo6I97arkQ6umeuYMqhIZ+WL65eHiH0cAEruUlXmD14hpvBLaeVD4WvBtGcS97/0W/2JcnsdnRm+30RWPtwzYkqT3BKMAoxYS2sIZH07jpa68fAsbbs9y0GXnFV7WoX/mZJ7g3TOLla/tDYmApK8sUnc6iHzIaL9lvGxvUgXXhUbONYB5/HxqAlTzBSsgATBLwr05PHJGg9WivJzoStOpAhaVlJ5zP+xnZGzLoZ9ZmLmBgd6QXL51bvC/aqNYmbGTRtUmdFZbW7HZilgAjSzSwDZdZPySZDCbDZ9W9C/jDjOwbkCGH9YyxgdhuV2mZDSOCZD5FMighr3w2JPaKgdriEYU8ulZ8g82em3jha8jtltTsttRb/Izpb14RemgKnzcFZ0yILhhxxrGFplYnQf2FmVe0eHEUohh1vkRJUG35gEkQ3ovQ9HUDCguewLdCrZ1Szadz5H7jZ9TxErFNf5RvvAaJUBF0zZFZOLxDq/agtCsO3mKdDsO6MP/5EgHcnOr0oSTryFs3IIsLqCVl0vUF/B1vd2WveP3nS2RM2CjYxwVEeGdyL/AyL5tob1nalPHp/N59OdZwRp0uaOU6/vJoI8Gic/iZv3P0bRartG+OIL2fPpxRCm9IHu9vlI0yvRu+3SF0DJbqrBjOPhcUrJce65GJITqPl2oSyzylHOtvQkcqyTi6R4VnllSFMX+IgfF83iVTQ83uemnb90LHRancxwSFa8KYzM9MwFghbzZHjv9XvbS22jFeCfsYP3NeV/YaxWQyE1Ar5D1VRUzolan2tReVRDadfjCPP+tdKRbYFOc02iOyTiRTN0gXFwueUxITOv28CRPW6PGRVSdNFQq+jF+i2x9gR3xMJkPx7tZXEazKN6XnSAlFZEsV8ubNyvpEGsrwcOPdbbHIdfFfvBzP8U3pdDo5OZDvx5/26xcvl4cypqTM19xWTFlVFfKDyHRW4qleGbw2nNWcoE7Eq60tJ0+SqpV6gRy0tJxsOdaCTwexo3h1vjdABAFbO0e+aRuMaPBbBLEgGkQDInAQc3Z+F2wsyd6kUKkUKiRiL6xQ4B9RqYoqFApVyfybRVFRTRF7D2rCmSo55t7WbyIqIiqaR8bWn89+Z1Nl5bTcog/OR+Xn3owpik1p/ysKsXVlNtWQr+62b4+7F8eIys5Nmfx67qAF7WNi8p+Ny4i8U5kfc3zQzhdgOCybTXF36hvJk+9Pvc+YX5KxYuB3x2MZGePGPJ6fvGJb8y1GCSOlWQW3dsP6rGrO3t44qHFV2kexq1b/9Gby8adHZcTl3onb1Hxr+zjG6nMaLHxLbMjex1RpkRUjd/bZmTayMZ+Rrekz5PTW9gURkbdUUZEVKWNGgd0wbNfE07vgF+FTmia4rR45O0qlOX0KrG9qAifCGFwPTjyLYd2x1pdAGwySlwbEEERjAzVOECTuD8IgpkFC2BEn1l04wx47Q9jC7tkDw7nwf2Vp/4lhR2l1hx9e2Y7SBp5zOp2H/304nbZdle/R/vz01wMHRj9ELBu97OP3/xFgAEAgj7rRci3dAAAAAElFTkSuQmCC");
                }
                if (postData.Entity.InteriorAfterImgURL2 != "../Images/mcd_logo.png")
                {
                    pdfData.Add("InteriorAfterImgURL2", HtmlConversionUtility.ImageToBase64(postData.Entity.InteriorAfterImgURL2, System.Drawing.Imaging.ImageFormat.Png));
                }
                else
                {
                    pdfData.Add("InteriorAfterImgURL2", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADYAAAAzCAMAAADrVgtcAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6REFDNzNCMkEwQkYyMTFFNEI5MzU4NkQ4NkQxNUM0MTYiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6REFDNzNCMkIwQkYyMTFFNEI5MzU4NkQ4NkQxNUM0MTYiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDpEQUM3M0IyODBCRjIxMUU0QjkzNTg2RDg2RDE1QzQxNiIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDpEQUM3M0IyOTBCRjIxMUU0QjkzNTg2RDg2RDE1QzQxNiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pnx0qhIAAAMAUExURbQADfK1A/rF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");
                }
                string pdfPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.ReopenMemo, pdfData, null);
                EmailSendingResultType result;
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("StoreCode", postData.Entity.Store.StoreBasicInfo.StoreCode);
                bodyValues.Add("StoreName", postData.Entity.Store.StoreBasicInfo.NameENUS);
                bodyValues.Add("Actor", actor.RoleNameENUS);////--呈递人
                if (postData.Entity.ProjectId.ToLower().IndexOf("rebuild") >= 0)
                {
                    bodyValues.Add("WorkflowName", Constants.Rebuild_ReopenMemo); ////--流程名称
                    bodyValues.Add("ProjectName", Constants.Rebuild); //项目名称
                }
                else if (postData.Entity.ProjectId.ToLower().IndexOf("reimage") >= 0)
                {
                    bodyValues.Add("WorkflowName", Constants.Reimage_ReopenMemo); ////--流程名称
                    bodyValues.Add("ProjectName", Constants.Reimage); //项目名称
                }
                else if (postData.Entity.ProjectId.ToLower().IndexOf("tempclosure") >= 0)
                {
                    bodyValues.Add("WorkflowName", Constants.TempClosure_ReopenMemo); ////--流程名称
                    bodyValues.Add("ProjectName", Constants.TempClosure); //项目名称
                }
                else if (postData.Entity.ProjectId.ToLower().IndexOf("majorlease") >= 0)
                {
                    bodyValues.Add("WorkflowName", Constants.MajorLease_ReopenMemo); ////--流程名称
                    bodyValues.Add("ProjectName", Constants.MajorLease); //项目名称
                }
                else if (postData.Entity.ProjectId.ToLower().IndexOf("renewal") >= 0)
                {
                    bodyValues.Add("WorkflowName", Constants.Renewal_ReopenMemo); ////--流程名称
                    bodyValues.Add("ProjectName", Constants.Renewal); //项目名称
                }
                else
                {
                    bodyValues.Add("WorkflowName", Constants.TempClosure_ReopenMemo);////--流程名称
                    bodyValues.Add("ProjectName", Constants.TempClosure);//项目名称
                }

                string viewPage = "";
                if (postData.Entity.ProjectId.ToLower().IndexOf("rebuild") >= 0)
                {
                    viewPage = string.Format("{0}/Rebuild/Main#/ReopenMemo/Process/View?projectId={1}",
                       ConfigurationManager.AppSettings["webHost"], postData.Entity.ProjectId);
                }
                if (postData.Entity.ProjectId.ToLower().IndexOf("reimage") >= 0)
                {
                    viewPage = string.Format("{0}/Reimage/Main#/ReopenMemo/Process/View?projectId={1}",
                        ConfigurationManager.AppSettings["webHost"], postData.Entity.ProjectId);
                }
                bodyValues.Add("FormUrl", viewPage);

                //调用邮件服务发送邮件
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    EmailMessage message = new EmailMessage();
                    StringBuilder sbTo = new StringBuilder();
                    Dictionary<string, string> attachments = new Dictionary<string, string>();
                    foreach (Employee emp in postData.Receivers)
                    {
                        if (sbTo.Length > 0)
                        {
                            sbTo.Append(";");
                        }
                        if (!string.IsNullOrEmpty(emp.Mail))
                        {
                            sbTo.Append(emp.Mail);
                        }
                    }
                    if (sbTo.Length > 0)
                    {
                        sbTo.Append(";");
                    }
                    message.EmailBodyValues = bodyValues;
                    string strTitle = "";
                    if (postData.Entity.ProjectId.ToLower().IndexOf("rebuild") >= 0)
                    {
                        strTitle = "Rebuild_ReopenMemo";
                    }
                    if (postData.Entity.ProjectId.ToLower().IndexOf("reimage") >= 0)
                    {
                        strTitle = "Reimage_ReopenMemo";
                    }
                    if (postData.Entity.ProjectId.ToLower().IndexOf("majorlease") >= 0)
                    {
                        strTitle = "MajorLease_ReopenMemo";
                    }
                    if (postData.Entity.ProjectId.ToLower().IndexOf("renewal") >= 0)
                    {
                        strTitle = "Renewal_ReopenMemo";
                    }
                    if (postData.Entity.ProjectId.ToLower().IndexOf("tempclosure") >= 0)
                    {
                        strTitle = "TempClosure_ReopenMemo";
                    }
                    attachments.Add(pdfPath, strTitle + "_" + postData.Entity.ProjectId + ".pdf");
                    message.AttachmentsDict = attachments;
                    message.To = sbTo.ToString();
                    message.TemplateCode = EmailTemplateCode.GBMemoNotification;
                    result = client.SendNotificationEmail(message);
                }

                if (!result.Successful)
                {
                    return BadRequest(result.ErrorMessage + " " + pdfPath);
                }
                ReopenMemo.Submit(postData.Entity);
                tranScope.Complete();
            }
            return Ok();
        }

        private string GetConcetpetDesc(string code)
        {
            var list = Dictionary.Search(e => e.ParentCode == "DesignType").ToList();
            string retrunValue = "";
            if (string.IsNullOrEmpty(code))
                return "";
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.Code == code)
                    {
                        retrunValue = item.NameENUS;
                        break;
                    }
                }
            }
            return retrunValue;
        }

        [Route("api/ReopenMemo/QuerySaveable")]
        [HttpGet]
        public IHttpActionResult QuerySaveable(string projectId)
        {
            string flowCode = "";
            if (projectId.ToLower().IndexOf("rebuild") != -1)
                flowCode = FlowCode.Rebuild_ReopenMemo;
            return Ok(new
            {
                IsShowSave = ProjectInfo.IsFlowSavable(projectId, flowCode)
            });
        }

        [Route("api/ReopenMemo/GetSelectYearMonth")]
        [HttpGet]
        public IHttpActionResult GetSelectYearMonth(string usCode)
        {
            var financeYear = ReopenMemo.GetSelectYearMonth(usCode);
            return Ok(financeYear);
        }

    }
}
