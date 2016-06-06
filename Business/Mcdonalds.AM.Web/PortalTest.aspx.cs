using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mcdonalds.AM.DataAccess;
using System.Configuration;
using Mcdonalds.AM.Web.Core;

namespace Mcdonalds.AM.Web
{
    public partial class PortalTest : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkBtn_Logout.Click += lnkBtn_Logout_Click;

            if (!Page.IsPostBack)
            {
                UserInfo _currEmp = McdAMContext.CurrentUser;

                lbUser.Text = string.Format("{0} {1}({2})", _currEmp.Code, _currEmp.NameZHCN, _currEmp.NameENUS);

                foreach (TreeNode node in tvMenu.Nodes)
                {
                    TraversalNodes(node);
                }

            }

            
        }

        private void TraversalNodes(TreeNode node)
        {
            string _navURL = node.NavigateUrl;

            if (_navURL != "")
            {
                if (_navURL.Contains("Fx.WebHostUri"))
                {
                    _navURL = _navURL.Replace("Fx.WebHostUri", ConfigurationManager.AppSettings["Fx.WebHostUri"]);
                }
                node.NavigateUrl = _navURL + "?user-id=" + Request["user-id"];
            }

            if (node.ChildNodes.Count > 0)
            {
                foreach (TreeNode cnode in node.ChildNodes)
                {
                    TraversalNodes(cnode);
                }
            }
        }


        void lnkBtn_Logout_Click(object sender, EventArgs e)
        {
            McdAMContext.ClearUser();
            Response.Redirect("LoginTest.aspx");
        }

    }
}