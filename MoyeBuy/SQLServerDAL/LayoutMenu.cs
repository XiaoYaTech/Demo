using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.Model;

namespace MoyeBuy.Com.SQLServerDAL
{
    public class LayoutMenu : DALBase, IDAL.ILayoutMenu
    {
        public string AddUpdateMenu(IList<Model.Menu> listMenu)
        {
            string strMenuIDs = "";
            string strMenuTypes = "";
            string strIsAdminMenus = "";
            string strMenuNames = "";
            string strMenuURLs = "";
            string strMenuDisqs = "";
            string strMenuTargets = "";
            string strMenuClassNames = "";
            string strMenuControlIDs = "";
            string strMenuTitles = "";
            string strReturn = "";
            Hashtable hshParam = new Hashtable();
            try
            {
                foreach (Menu menu in listMenu)
                {
                    strMenuIDs += menu.MenuID + "|||";
                    strMenuTypes += menu.MenuType + "|||";
                    strIsAdminMenus += menu.IsAdminMenu + "|||";
                    strMenuNames += menu.MenuName + "|||";
                    strMenuURLs += menu.MenuUrl + "|||";
                    strMenuDisqs += menu.Disq + "|||";
                    strMenuTargets += menu.Target + "|||";
                    strMenuClassNames += menu.MenuClassName + "|||";
                    strMenuControlIDs += menu.MenuControlID + "|||";
                    strMenuTitles += menu.MenuTitle + "|||";
                }
                if (strMenuIDs.Length > 3)
                    strMenuIDs = strMenuIDs.Substring(0, strMenuIDs.Length - 3);

                if (strMenuTypes.Length > 3)
                    strMenuTypes = strMenuTypes.Substring(0, strMenuTypes.Length - 3);

                if (strIsAdminMenus.Length > 3)
                    strIsAdminMenus = strIsAdminMenus.Substring(0, strIsAdminMenus.Length - 3);

                if (strMenuNames.Length > 3)
                    strMenuNames = strMenuNames.Substring(0, strMenuNames.Length - 3);

                if (strMenuURLs.Length > 3)
                    strMenuURLs = strMenuURLs.Substring(0, strMenuURLs.Length - 3);

                if (strMenuDisqs.Length > 3)
                    strMenuDisqs = strMenuDisqs.Substring(0, strMenuDisqs.Length - 3);

                if (strMenuTargets.Length > 3)
                    strMenuTargets = strMenuTargets.Substring(0, strMenuTargets.Length - 3);

                if (strMenuClassNames.Length > 3)
                    strMenuClassNames = strMenuClassNames.Substring(0, strMenuClassNames.Length - 3);

                if (strMenuControlIDs.Length > 3)
                    strMenuControlIDs = strMenuControlIDs.Substring(0, strMenuControlIDs.Length - 3);

                if (strMenuTitles.Length > 3)
                    strMenuTitles = strMenuTitles.Substring(0, strMenuTitles.Length - 3);

                Gadget.Addparamater(ref hshParam, "MenuIDs", strMenuIDs);
                Gadget.Addparamater(ref hshParam, "MenuTypes", strMenuTypes);
                Gadget.Addparamater(ref hshParam, "IsAdminMenus", strIsAdminMenus);
                Gadget.Addparamater(ref hshParam, "MenuNames", strMenuNames);
                Gadget.Addparamater(ref hshParam, "MenuURLs", strMenuURLs);
                Gadget.Addparamater(ref hshParam, "MenuDisqs", strMenuDisqs);
                Gadget.Addparamater(ref hshParam, "MenuTargets", strMenuTargets);
                Gadget.Addparamater(ref hshParam, "MenuClassNames", strMenuClassNames);
                Gadget.Addparamater(ref hshParam, "MenuControlIDs", strMenuControlIDs);
                Gadget.Addparamater(ref hshParam, "MenuTitles", strMenuTitles);
                DataSet dsRsult = dbOperator.ProcessData("usp_AddUpdateMoyeBuyMenu", hshParam, strDSN);
                
                if (Gadget.DatatSetIsNotNullOrEmpty(dsRsult))
                {
                    strReturn = dsRsult.Tables[0].Rows[0]["NewMenuIDs"].ToString();
                }
            }
            catch (Exception ex)
            {
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.LayoutMenu.AddUpdateMenu()", UtilityFactory.LogType.LogToFile);
            }
            return strReturn;
        }

        public IList<Model.Menu> GetMenuData(string strMenuTyp, bool IsAdmin)
        {
            IList<Menu> listMenu = null;
            ArrayList arrSubMenuID = new ArrayList();
            DataSet dsMenu = GetDataSetMenuData(strMenuTyp, IsAdmin);

            if (Gadget.DatatSetIsNotNullOrEmpty(dsMenu))
            {
                listMenu = new List<Menu>();
                string strTempMenuId = "";

                DataView dview = dsMenu.Tables[0].DefaultView;
                foreach (DataRow drmenu in dsMenu.Tables[0].Rows)
                {
                    string strTemSub = Gadget.GetDataRowStringValue(drmenu, "SubMenuID");
                    if (!string.IsNullOrEmpty(strTemSub))
                        arrSubMenuID.Add(Gadget.GetDataRowStringValue(drmenu, "SubMenuID"));
                }
                foreach (DataRow drmenu in dsMenu.Tables[0].Rows)
                {
                    if (strTempMenuId != Gadget.GetDataRowStringValue(drmenu, "MenuID") && !arrSubMenuID.Contains(Gadget.GetDataRowStringValue(drmenu, "MenuID")))
                    {
                        string strAllTempID = "";
                        strTempMenuId = Gadget.GetDataRowStringValue(drmenu, "MenuID");
                        dview.RowFilter = "MenuID = '" + strTempMenuId + "'";
                        foreach (DataRow dr in dview.ToTable().Rows)
                        {
                            string strTemSub = Gadget.GetDataRowStringValue(dr, "SubMenuID");
                            if (!string.IsNullOrEmpty(strTemSub))
                            {
                                strAllTempID += strTemSub + "|";
                            }
                        }
                        if (strAllTempID.Length > 0)
                            strAllTempID = strAllTempID.Substring(0, strAllTempID.Length - 1);
                        listMenu.Add(GetMenu(drmenu, dsMenu.Copy(), strAllTempID));
                    }
                }
            }
            return listMenu;
        }

        public bool MappingSubMenu(string strMenuMappingIDs, string strMainMenuID, string strSubMenuIDs)
        {
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "SubMenuMappingIDs", strMenuMappingIDs);
            Gadget.Addparamater(ref hshParam, "MenuID", strMainMenuID);
            Gadget.Addparamater(ref hshParam, "SubMenuIDs", strSubMenuIDs);
            DataSet dsRsult = dbOperator.ProcessData("usp_AddUpdateSubMenuMapping", hshParam, strDSN);
            return Gadget.DatatSetIsNotNullOrEmpty(dsRsult);
        }

        private string GetSubMenuID(DataRowCollection rows)
        {
            string strAllTempID = "";
            foreach (DataRow drMenu in rows)
            {
                string strTemSub = Gadget.GetDataRowStringValue(drMenu, "SubMenuID");
                if (!string.IsNullOrEmpty(strTemSub) && !strAllTempID.Contains(strTemSub))
                {
                    strAllTempID += strTemSub + "|";
                }
            }
            if (strAllTempID.Length > 0)
                strAllTempID = strAllTempID.Substring(0, strAllTempID.Length - 1);
            return strAllTempID;
        }
        private IList<Menu> GetSubMenuData(DataSet dsMenu, string strSubMenuId)
        {
            IList<Menu> listMenu = null;
            if (Gadget.DatatSetIsNotNullOrEmpty(dsMenu))
            {
                listMenu = new List<Menu>();
                string strTempMenuId = "";
                IList<String> listSubMenuId = Gadget.Split<String>(strSubMenuId, "|");
                if (listSubMenuId != null)
                {
                    strSubMenuId = "MenuID IN (";
                    foreach (string strTemp in listSubMenuId)
                    {
                        strSubMenuId += strTemp + ",";
                    }
                    strSubMenuId = strSubMenuId.Substring(0, strSubMenuId.Length - 1);
                    strSubMenuId += ")";
                }
                DataView dview = dsMenu.Tables[0].DefaultView;
                if (!String.IsNullOrEmpty(strSubMenuId))
                    dview.RowFilter = strSubMenuId;
                foreach (DataRow drmenu in dview.ToTable().Rows)
                {
                    if (strTempMenuId != Gadget.GetDataRowStringValue(drmenu, "MenuID"))
                    {
                        strTempMenuId = Gadget.GetDataRowStringValue(drmenu, "MenuID");
                        string strNextSubMenuId = GetSubMenuID(dview.ToTable().Rows);
                        listMenu.Add(GetMenu(drmenu, dsMenu.Copy(), strNextSubMenuId));
                    }
                }
            }
            return listMenu;
        }

        private Menu GetMenu(DataRow drmenu, DataSet dsMenu, string strSubMenuId)
        {
            Menu menu = new Menu();
            menu.MenuID = Gadget.GetDataRowIntValue(drmenu, "MenuID");
            menu.Disq = Gadget.GetDataRowStringValue(drmenu, "MenuDisq");
            menu.MenuUrl = Gadget.GetDataRowStringValue(drmenu, "MenuURL");
            menu.Target = Gadget.GetDataRowStringValue(drmenu, "MenuTarget");
            menu.MenuName = Gadget.GetDataRowStringValue(drmenu, "MenuName");
            menu.MenuType = Gadget.GetDataRowStringValue(drmenu, "MenuType");
            menu.IsAdminMenu = Gadget.GetDataRowBoolValue(drmenu, "IsAdminMenu");
            menu.MenuClassName = Gadget.GetDataRowStringValue(drmenu, "MenuClassName");
            menu.MenuControlID = Gadget.GetDataRowStringValue(drmenu, "MenuControlID");
            menu.MenuTitle = Gadget.GetDataRowStringValue(drmenu, "MenuTitle");
            if (!string.IsNullOrEmpty(strSubMenuId))
                menu.SubMenu = GetSubMenuData(dsMenu, strSubMenuId);

            return menu;
        }
        private DataSet GetDataSetMenuData(string strMenuTyp, bool IsAdmin)
        {
            DataSet dsMenu = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "MenuType", strMenuTyp);
            Gadget.Addparamater(ref hshParam, "IsAdminMenu", IsAdmin ? "1" : "0");
            dsMenu = dbOperator.ProcessData("usp_GetMoyeBuyLayoutMenu", hshParam, strDSN);
            return dsMenu;
        }
    }
}
