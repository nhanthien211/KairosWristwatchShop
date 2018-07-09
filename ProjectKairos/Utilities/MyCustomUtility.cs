<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
=======
﻿using System.Web;
>>>>>>> origin/backend_admin_feature_nhan

namespace ProjectKairos.Utilities
{
    public class MyCustomUtility
    {
        public static string RelativeFromAbsolutePath(string path)
        {
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                var applicationPath = request.PhysicalApplicationPath;
                var virtualDir = request.ApplicationPath;
                virtualDir = virtualDir == "/" ? virtualDir : (virtualDir + "/");
                return path.Replace(applicationPath, virtualDir).Replace(@"\", "/");
            }
            return "";
        }
<<<<<<< HEAD
=======


>>>>>>> origin/backend_admin_feature_nhan
    }
}