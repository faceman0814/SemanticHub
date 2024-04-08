
//using Aspose.Pdf;

//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.tool.xml;
//using iTextSharp.tool.xml.html;

//using System.Text;
//using System.Xml.Linq;

//using Document = iTextSharp.text.Document;

//namespace FaceMan.SemanticHub.Helper.HtmlHelper
//{
//    /// <summary>
//	/// HTML帮助类
//	/// </summary>
//	public class HtmlHelper
//    {

//        /// <summary>
//        /// 字体路径
//        /// </summary>
//        private static string FontPath;

//        /// <summary>
//        /// 图片XY字典，例如 [{img1, 100,200},{img2,20,30}}
//        /// </summary>
//        private Dictionary<string, Tuple<float, float>> m_ImageXYDic = null;

//        public HtmlHelper(string fontPath)
//        {
//            FontPath = fontPath;
//        }

//        public HtmlHelper()
//        {
//        }

//        public HtmlHelper(string htmlString, string fontPath, Dictionary<string, Tuple<float, float>> imageXYDic)
//        {
//            FontPath = fontPath;
//            m_ImageXYDic = imageXYDic;
//        }

//        //将html字符串转为字节数组（代码来自百度）
//        public byte[] ConvertHtmlTextToPDF(string htmlText)
//        {
//            if (string.IsNullOrEmpty(htmlText))
//            {
//                return null;
//            }

//            try
//            {
//                MemoryStream outputStream = new MemoryStream(); //要把PDF寫到哪個串流
//                byte[] data = Encoding.UTF8.GetBytes(htmlText); //字串轉成byte[]
//                MemoryStream msInput = new MemoryStream(data);
//                Document doc = new Document(); //要寫PDF的文件，建構子沒填的話預設直式A4
//                PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);

//                //指定文件預設開檔時的縮放為100%
//                PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
//                //開啟Document文件 
//                doc.Open();

//                #region 图片的处理
//                //CssFilesImpl cssFiles = new CssFilesImpl();
//                //cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS());
//                //var cssResolver = new StyleAttrCSSResolver(cssFiles);

//                //var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
//                //tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
//                //tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor(m_ImageXYDic)); // use new processor

//                //var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
//                //hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors

//                //var charset = Encoding.UTF8;
//                //var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(doc, writer));
//                //var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
//                //var worker = new XMLWorker(pipeline, true);
//                //var xmlParser = new XMLParser(true, worker, charset);
//                //xmlParser.Parse(new StringReader(htmlText));
//                #endregion

//                //使用XMLWorkerHelper把Html parse到PDF檔裡
//                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new UnicodeFontFactory());

//                //將pdfDest設定的資料寫到PDF檔
//                PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
//                writer.SetOpenAction(action);

//                doc.Close();
//                msInput.Close();
//                outputStream.Close();

//                return outputStream.ToArray();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("转PDF时异常,请联系管理员！", ex);
//            }
//        }

//        public byte[] ConvertHtmlTextToPDF1(string htmlText)
//        {
//            if (string.IsNullOrEmpty(htmlText))
//            {
//                return null;
//            }
//            //避免當htmlText無任何html tag標籤的純文字時，轉PDF時會掛掉，所以一律加上<p>標籤
//            //htmlText = "<p>" + htmlText + "</p>";

//            try
//            {

//                MemoryStream outputStream = new MemoryStream(); //要把Word寫到哪個串流
//                byte[] data = Encoding.UTF8.GetBytes(htmlText); //字串轉成byte[]
//                MemoryStream stream = new MemoryStream(data);
//                Aspose.Pdf.Document doc = new Aspose.Pdf.Document(stream, new HtmlLoadOptions()); ; //要寫Word的文件，建構子沒填的話預設直式A4
//                doc.Save(outputStream);
//                return outputStream.ToArray();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("转PDF时异常,请联系管理员！", ex);
//            }
//        }

//        public byte[] ConvertHtmlTextToWord(string htmlText)
//        {
//            if (string.IsNullOrEmpty(htmlText))
//            {
//                return null;
//            }
//            //避免當htmlText無任何html tag標籤的純文字時，轉Word時會掛掉，所以一律加上<p>標籤
//            //htmlText = "<p>" + htmlText + "</p>";

//            try
//            {
//                MemoryStream outputStream = new MemoryStream(); //要把Word寫到哪個串流
//                byte[] data = Encoding.UTF8.GetBytes(htmlText); //字串轉成byte[]
//                MemoryStream stream = new MemoryStream(data);
//                Aspose.Words.Document doc = new Aspose.Words.Document(stream, new Aspose.Words.Loading.LoadOptions()); //要寫Word的文件，建構子沒填的話預設直式A4
//                doc.Save(outputStream, (Aspose.Words.SaveFormat)SaveFormat.Doc);
//                //Spire.Doc.Document doc = new Spire.Doc.Document(); //要寫Word的文件，建構子沒填的話預設直式A4
//                //doc.LoadFromStream(stream, FileFormat.Html, XHTMLValidationType.None);
//                //doc.SaveToStream(outputStream, FileFormat.Docx);
//                //doc.Close();
//                return outputStream.ToArray();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("转Word时异常,请联系管理员！", ex);
//            }
//        }
//        //字体工厂（代码来自百度）
//        public class UnicodeFontFactory : FontFactoryImp
//        {
//            public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color,
//                bool cached)
//            {
//                BaseFont baseFont = BaseFont.CreateFont(FontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
//                return new Font(baseFont, size, style, color);
//            }
//        }

//        //自定义的图片处理类（代码来自百度）
//        public class CustomImageTagProcessor : iTextSharp.tool.xml.html.Image
//        {
//            //个人加入的图片位置处理代码
//            private float _offsetX = 0;
//            private float _offsetY = 0;

//            private Dictionary<string, Tuple<float, float>> _imageXYDict;//个人加入的图片位置处理代码
//            public CustomImageTagProcessor(Dictionary<string, Tuple<float, float>> imageXYDict)//个人加入的图片位置处理代码
//            {
//                _imageXYDict = imageXYDict;
//            }

//            protected void SetImageXY(string imageId)//个人加入的图片位置处理代码
//            {
//                if (_imageXYDict == null)
//                {
//                    return;
//                }
//                Tuple<float, float> xyTuple = null;
//                _imageXYDict.TryGetValue(imageId, out xyTuple);

//                if (xyTuple != null)
//                {
//                    _offsetX = xyTuple.Item1;
//                    _offsetY = xyTuple.Item2;
//                }
//            }

//            public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
//            {
//                IDictionary<string, string> attributes = tag.Attributes;
//                string src;
//                if (!attributes.TryGetValue(HTML.Attribute.SRC, out src))
//                    return new List<IElement>(1);

//                if (string.IsNullOrEmpty(src))
//                    return new List<IElement>(1);

//                string imageId;//个人加入的图片位置处理代码
//                if (!attributes.TryGetValue(HTML.Attribute.ID, out imageId))//个人加入的图片位置处理代码
//                    return new List<IElement>(1);

//                if (string.IsNullOrEmpty(imageId))
//                    return new List<IElement>(1);

//                SetImageXY(imageId);//个人加入的图片位置处理代码

//                if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
//                {
//                    // data:[][;charset=][;base64],
//                    var base64Data = src.Substring(src.IndexOf(",") + 1);
//                    var imagedata = Convert.FromBase64String(base64Data);
//                    var image = iTextSharp.text.Image.GetInstance(imagedata);

//                    var list = new List<IElement>();
//                    var htmlPipelineContext = GetHtmlPipelineContext(ctx);
//                    list.Add(GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers().Apply(image, tag, htmlPipelineContext), _offsetX, _offsetY, true), tag, htmlPipelineContext));
//                    return list;
//                }
//                else
//                {
//                    return base.End(ctx, tag, currentContent);
//                }
//            }
//        }
//    }
//}
