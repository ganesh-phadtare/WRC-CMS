using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Communication;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class BaseController : Controller
    {
        public async Task<string> BaseAddUpdateRecord(ICommon CommonModelObjet, ModelStateDictionary ModelState, WebApiProxy proxy)
        {
            if (ModelState.IsValid)
            {
                int ViewID = 0;
                await Task.Run(() =>
                {
                    if (CommonModelObjet.CurrentObjectId == 0)
                        ViewID = BORepository.AddUpdateRecord<ICommon>(proxy, CommonModelObjet, true).Result;
                    else
                        ViewID = BORepository.AddUpdateRecord<ICommon>(proxy, CommonModelObjet, false).Result;
                });

                if (ViewID > 0)
                    return "Record Saved successfully.";
            }

            return "Problem occured while saving record, kindly contact our support team.";
        }

        public string BaseDeleteRecord(ICommon CommonmodelObject,ModelStateDictionary ModelState,WebApiProxy proxy)
        {
            int Count = 0;
            if (ModelState.IsValid)
            {
                Count = BORepository.Delete<ICommon>(proxy, CommonmodelObject);

                if (Count >0)
                    return "Record Saved successfully.";
            }
            return "Problem occured while saving record, kindly contact our support team.";
        }
    }
}