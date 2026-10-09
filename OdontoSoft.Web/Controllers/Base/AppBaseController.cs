using MedveCore.Common.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OdontoSoft.Web.Controllers
{
    public class AppBaseController : Controller
    {
        protected JsonResult JsonOk()
        {
            return Json(new MedveJSON(true, "", "", null));
        }

        protected JsonResult JsonOk(string dataLabel, object dataContent)
        {
            return Json(new MedveJSON(true, "", dataLabel, dataContent));
        }

        protected JsonResult JsonListDropDown(object dataContent)
        {
            return Json(new MedveJSON(true, "", "Data", dataContent));
        }

        protected JsonResult JsonOk(object dataContent)
        {
            return Json(new MedveJSON(true, "", dataContent));
        }

        protected JsonResult JsonError(string message)
        {
            return Json(new MedveJSON(false, message));
        }

        protected JsonResult JsonError(object errorContent)
        {
            return Json(new MedveJSON(false, "", errorContent));
        }


        protected bool IsJson
        {
            get
            {



                return !String.IsNullOrEmpty(HttpContext.Request.Form["json"])
                    || !String.IsNullOrEmpty(HttpContext.Request.Query["json"]);

            }
        }

        //public bool IsReadOnly
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (System.Web.HttpContext.Current.Items["crudReadOnly"] == null)
        //            {
        //                return false;
        //            }
        //            else
        //            {
        //                return Converter.ToBoolean(System.Web.HttpContext.Current.Items["crudReadOnly"]);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return false;
        //        }
        //    }
        //}


        protected void AddModelErrors(ModelStateDictionary modelState)
        {
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

            modelState.AddModelError(String.Empty, messages);
            //modelState.AddModelError(ve.PropertyName, ve.ErrorMessage);


        }


        //protected void AddModelErrors(ModelStateDictionary modelState, DbEntityValidationException ex)
        //{

        //    foreach (var eve in ex.EntityValidationErrors)
        //    {
        //        foreach (var ve in eve.ValidationErrors)
        //        {
        //            modelState.AddModelError(String.Empty, String.Format("{0} -> {1} -> {2}", eve.Entry.Entity.GetType().Name, ve.PropertyName, ve.ErrorMessage));
        //            modelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
        //        }

        //        //Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
        //        //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //        //foreach (var ve in eve.ValidationErrors)
        //        //{
        //        //    //this.ModelState.AddModelError("", ex);
        //        //    Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
        //        //        ve.PropertyName, ve.ErrorMessage);
        //        //}
        //    }

        //    //if (ex.ValidationEntitiesItems != null && ex.ValidationEntitiesItems.Count > 0)
        //    //{
        //    //    if (ex.ValidationEntitiesItems.Count == 1)
        //    //    {
        //    //        foreach (var property in ex.ValidationEntitiesItems[0].ValidationPropertyItems)
        //    //        {
        //    //            modelState.AddModelError(property.PropertyName, property.ValidationMessage);
        //    //        }
        //    //    }
        //    //    else
        //    //    {

        //    //        foreach (var entity in ex.ValidationEntitiesItems)
        //    //        {
        //    //            foreach (var property in ex.ValidationEntitiesItems[0].ValidationPropertyItems)
        //    //            {
        //    //                modelState.AddModelError("", property.PropertyName + " en " + entity.EntityName + ": " + property.ValidationMessage);
        //    //            }
        //    //        }
        //    //    }

        //    //}
        //    //else
        //    //{
        //    //    modelState.AddModelError("", ex.Message);
        //    //}

        //}


        //protected void AddModelErrors(ModelStateDictionary modelState, Service.Base.MedveValidationException ex)
        //{

        //    if (ex.ValidationEntitiesItems != null && ex.ValidationEntitiesItems.Count > 0)
        //    {
        //        if (ex.ValidationEntitiesItems.Count == 1)
        //        {
        //            foreach (var property in ex.ValidationEntitiesItems[0].ValidationPropertyItems)
        //            {
        //                modelState.AddModelError(property.PropertyName, property.ValidationMessage);
        //            }
        //        }
        //        else
        //        {

        //            foreach (var entity in ex.ValidationEntitiesItems)
        //            {
        //                foreach (var property in ex.ValidationEntitiesItems[0].ValidationPropertyItems)
        //                {
        //                    modelState.AddModelError("", property.PropertyName + " en " + entity.EntityName + ": " + property.ValidationMessage);
        //                }
        //            }
        //        }

        //    }
        //    else
        //    {
        //        modelState.AddModelError("", ex.Message);
        //    }

        //}


        protected bool ValidateHash(object value, string hash)
        {
            return String.Equals(HashString(value.ToString()), hash, StringComparison.OrdinalIgnoreCase);
        }

        protected string HashString(string text/*, string salt = ""*/)
        {
            string salt = "triangulo";
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // Convert the string to a byte array first, to be processed
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }

    }
}