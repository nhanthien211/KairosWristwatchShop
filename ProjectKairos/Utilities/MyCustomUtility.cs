using System.Web;

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


    }
}