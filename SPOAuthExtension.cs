using System;
using System.Windows.Forms;
using Fiddler;

[assembly: Fiddler.RequiredVersion("4.4.5.1")]

namespace SPOAuthFiddlerExt {
  public class SPOAuthExtension : Inspector2, IRequestInspector2 {

    private bool _readOnly;
    HTTPRequestHeaders _headers;
    private byte[] _body;
    SPOAuthRequestControl _displayControl;

    #region Inspector2 implementation
    public override void AddToTab(TabPage o) {
        
      _displayControl = new SPOAuthRequestControl();
      o.Text = "SPOAuth";
      o.Controls.Add(_displayControl);
      o.Controls[0].Dock = DockStyle.Fill;
    }

    public override int GetOrder() {
      return 0;
    }
    #endregion


    #region IRequestInspector2 implementation
    public HTTPRequestHeaders headers {
      get {
        return _headers;
      }
      set {
          
        _headers = value;
        System.Collections.Generic.Dictionary<string, string> httpHeaders = new System.Collections.Generic.Dictionary<string, string>();
        foreach (var item in headers) {
          httpHeaders.Add(item.Name, item.Value);
        }
        _displayControl.HTTPHeaders = httpHeaders;
      }
    }

    public void Clear() {
      _displayControl.Clear();
    }

    public bool bDirty {
      get { return false; }
    }

    public bool bReadOnly {
      get {
        return _readOnly;
      }
      set {
        _readOnly = value;
      }
    }

    public byte[] body {
      get {
        return _body;
      }
      set {
        _body = value;
        _displayControl.HTTPBody = body;
      }
    }
    #endregion

  }

}