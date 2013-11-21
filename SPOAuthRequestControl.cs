using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;


namespace SPOAuthFiddlerExt {
  public partial class SPOAuthRequestControl : UserControl {
    public SPOAuthRequestControl() {
      InitializeComponent();

    }

    public Dictionary<string, string> Headers {
      set {
        txtContext.Text = string.Empty;
        foreach (string key in value.Keys) {
          if (key == "Authorization") {
            //Access token
            string accessToken = value[key].Replace("Bearer ", string.Empty);
            string token = JsonWebToken.Decode(accessToken)[1];
            txtContext.Text = token;
            var dictionary = JsonWebToken.Deserialize(token);

            PopulateGrid(dictionary);

          }
        }
      }
    }

    public byte[] Body {
      set {
        bool found = false;
        if (null != value) {
          string ret = System.Text.Encoding.UTF8.GetString(value);

          //Context token may be sent as a querystring with these names, but this is not
          //recommended practice and is not implemented here as a result.  Only POST values
          //are processed.
          string[] formParameters = ret.Split('&');
          string[] paramNames = { "AppContext", "AppContextToken", "AccessToken", "SPAppToken" };

          foreach (string valuePair in formParameters) {
            string[] formParameter = valuePair.Split('=');
            foreach (string paramName in paramNames) {
              if (formParameter[0] == paramName) {
                //Decode header of JWT token
                string tokenHeader = JsonWebToken.Decode(formParameter[1])[0];
                txtContext.Text = tokenHeader;

                //Decode body of JWT token
                string tokenBody = JsonWebToken.Decode(formParameter[1])[1];
                txtContext.Text += tokenBody;

                var dictionary = JsonWebToken.Deserialize(tokenBody);

                PopulateGrid(dictionary);

                found = true;
                break;
              }
            }
            if (found)
              break;
          }
        }

      }
    }



    public void Clear() {
      txtContext.Text = string.Empty;

    }

    private void PopulateGrid(IReadOnlyDictionary<string, object> dictionary) {
      dataGridView1.Rows.Clear();

      foreach (string key in dictionary.Keys) {
        if (key == "nbf" || key == "exp") {
          double d = double.Parse(dictionary[key].ToString());
          DateTime dt = new DateTime(1970, 1, 1).AddSeconds(d);
          dataGridView1.Rows.Add(key, dt);
        } else {
          dataGridView1.Rows.Add(key, dictionary[key]);
        }
      }
    }
  }
}
