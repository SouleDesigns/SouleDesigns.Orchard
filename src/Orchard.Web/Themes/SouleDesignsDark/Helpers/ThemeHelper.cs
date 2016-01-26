using System.Linq;
using System.Collections.Generic;
using System.Web.Caching;
using Orchard;

namespace SouleDesignsDark.Helpers {

    public static class ThemeHelper {
        /// <summary>
        /// Retrieves settings for the theme from Orchard's WorkContext.
        /// <param name="workContext">The WorkContext to retrieve the settings from.</param>
        /// <param name="itemName">The setting name to retrieve.</param>
        /// <returns>A boolean representing the current Theme Setting.</returns>
        public static bool SettingsEval(this WorkContext workContext, string itemName) {
            var returnValue = false;
            var context = workContext.HttpContext;
            if (context.Items[itemName] != null && (string)context.Items[itemName] == bool.TrueString) {
                returnValue = true;
            }
            return returnValue;
        }

        /// <summary>
        /// Returns the correct Bootstrap CSS class for the current Aside Zone configuration.
        /// </summary>
        /// <param name="asideClass">The current Aside Zone configuration.</param>
        /// <returns>A string representing the correct Boostrap CSS class for the zone configuration.</returns>
        public static string GetAsideCssClass(string asideClass) {
            switch (asideClass) {
                case "aside-1":
                case "aside-2":
                    return "col-md-9";
                case "aside-12":
                    return "col-md-6";
                default:
                    return "col-md-12";
            }
        }

        /// <summary>
        /// Returns the correct Bootstrap CSS class for the current Tripel Zone configuration.
        /// </summary>
        /// <param name="tripelClass">The current Tripel Zone configuration.</param>
        /// <returns>A string representing the correct Boostrap CSS class for the zone configuration.</returns>
        public static string GetTripelCssClass(string tripelClass) {
            switch (tripelClass) {
                case "tripel-12":
                case "tripel-23":
                case "tripel-13":
                    return "col-md-6";
                case "tripel-123":
                    return "col-md-4";
                default:
                    return "col-md-12";
            }
        }

        /// <summary>
        /// Returns the correct Bootstrap CSS class for the current FooterQuad Zone configuration.
        /// </summary>
        /// <param name="footerQuadClass">The current FooterQuad Zone configuration.</param>
        /// <returns>A string representing the correct Boostrap CSS class for the zone configuration.</returns>
        public static string GetFooterQuadCssClass(string footerQuadClass) {
            switch (footerQuadClass) {
                case "split-12":
                case "split-13":
                case "split-14":
                case "split-23":
                case "split-24":
                case "split-34":
                    return "col-md-6";
                case "split-123":
                case "split-124":
                case "split-134":
                case "split-234":
                    return "col-md-4";
                case "split-1234":
                    return "col-md-3";
                default:
                    return "col-md-12";
            }
        }

      
        /// <summary>
        /// Perform http request to get vimeo large thumbnail, stored in cache once received
        /// to avoid hitting vimeo each load
        /// </summary>
        /// <param name="vimeoURL"></param>
        /// <returns></returns>
        public static string GetVimeoPreviewImage(string vimeoURL)
        {
            try
            {
                string vimeoUrl = System.Web.HttpContext.Current.Server.HtmlEncode(vimeoURL);
                var cache = System.Web.HttpContext.Current.Cache;

                int pos = vimeoUrl.LastIndexOf("/");
                string videoID = vimeoUrl.Substring(pos + 1, 8);
                string videoKey = string.Format("Vimeo_{0}", videoID);
                string imageURL = string.Empty;
            
                // Attempt to get from cache
                object imageUrlObject = cache[videoKey];
            
                if(imageUrlObject == null) {            
                    var doc = new System.Xml.XmlDocument();
                    doc.Load("http://vimeo.com/api/v2/video/" + videoID + ".xml");
                    var root = doc.DocumentElement;
                    string vimeoThumb = root.FirstChild.SelectSingleNode("thumbnail_large").ChildNodes[0].Value;
                    imageURL = vimeoThumb;
                
                    // Add to cache
                    cache[videoKey] = imageURL;

                } else {
                    imageURL = imageUrlObject.ToString();
                
                }
                        
                return imageURL;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Helper to get a custom list of portfolio piece media based on the input content item, kludgey
        /// way to combine video and images but saves writing a custom module which I don't want to spend time
        /// on - tons of dynamic shit here to avoid Orchard compilation issues
        /// </summary>
        /// <param name="contentItem">Portfolio piece content item</param>
        /// <returns></returns>
        public static List<PortfolioPieceMedia> GetPortfolioPieceMedia(dynamic contentItem)
        {            
            // Retreive dynamic values from content 
            var route = contentItem.PortfolioPiece.AutoroutePart.Path;
            var mediaParts = contentItem.PortfolioPiece.PieceMedia.MediaParts;
            var vimeoUrl1 = contentItem.PortfolioPiece.VimeoUrl1.Value;
            var vimeoUrl1Index = contentItem.PortfolioPiece.VimeoUrl1Index.Value;
            var vimeoUrl2 = contentItem.PortfolioPiece.VimeoUrl2.Value;
            var vimeoUrl2Index = contentItem.PortfolioPiece.VimeoUrl2Index.Value;
            var media = new List<PortfolioPieceMedia>();

            // Get images
            foreach(var img in mediaParts)
            {
                media.Add(new PortfolioPieceMedia
                {
                    MediaType = "Image",
                    MediaUrl = img.MediaUrl,
                    Title = img.Title,
                    Caption = img.Caption
                });
            }
            
            // Process video one
            if (vimeoUrl1 != null && vimeoUrl1Index != null)
            {
                var vimeo1 = new PortfolioPieceMedia
                {
                    MediaType = "Vimeo",
                    MediaUrl = vimeoUrl1,
                    Title = string.Empty,
                    Caption = string.Empty
                };

                if ((int)vimeoUrl1Index > media.Count())
                {
                    media.Add(vimeo1);
                }
                else
                {
                    media.Insert((int)vimeoUrl1Index, vimeo1);
                }
            }

            // Process video two
            if (vimeoUrl2 != null && vimeoUrl2Index != null)
            {
                var vimeo2 = new PortfolioPieceMedia
                {
                    MediaType = "Vimeo",
                    MediaUrl = vimeoUrl2,
                    Title = string.Empty,
                    Caption = string.Empty
                };

                if ((int)vimeoUrl2Index > media.Count())
                {
                    media.Add(vimeo2);
                }
                else
                {
                    media.Insert((int)vimeoUrl2Index, vimeo2);
                }
            }

            return media;
        }
    }

    /// <summary>
    /// Portfolio Piece Media wrapper, allows combining images and 
    /// vidoes into a single collection for display on the portfolio
    /// piece page, neato!
    /// </summary>
    public class PortfolioPieceMedia
    {
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
    }

}