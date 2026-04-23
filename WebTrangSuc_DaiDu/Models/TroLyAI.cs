using System;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebTrangSuc_DaiDu.Models
{
    public class TroLyAI
    {
        private static string apiUrlOllama = "http://localhost:11434/api/generate";

        // ==========================================
        // 1. MẮT THẦN OLLAMA (TIẾNG ANH + DỊCH LÉN + JS TỰ TẠO MÃ)
        // ==========================================
        public static string SoiAnhTrangSuc(string base64Image)
        {
            // ÉP LUẬT MỚI: Tuyệt đối không nhắc gì đến MaSP ở đây nữa
            string heThongChiDao = @"You are a jewelry expert. Output ONLY a valid JSON. Language: ENGLISH.

            RULES:
            1. 'TenSP': A beautiful, creative short English name.
            2. 'MaDM': You MUST output EXACTLY ONE code: DC1, VT2, KT3, NV4, NB5, DAQUY, BOTRANGSUC, LT, ZOD.
            3. 'MoTa': Describe the design, color, and material in ENGLISH. EXACTLY 4 short phrases separated by the pipe character (|). DO NOT use newlines.

            EXAMPLE FORMAT (DO NOT COPY VALUES):
            {
                ""TenSP"": ""Gothic Heart Necklace"",
                ""MaDM"": ""DC1"",
                ""MoTa"": ""Silver chain | Red heart pendant | Skeleton hand design | Perfect for Halloween""
            }";

            var payload = new
            {
                model = "llava-phi3",
                prompt = "Analyze this image and output the JSON.",
                system = heThongChiDao,
                stream = false,
                images = new[] { base64Image }
            };

            string rawJson = GoiOllama(payload);

            // TÍCH HỢP TOOL DỊCH THUẬT: Dịch Tiếng Anh sang Tiếng Việt trước khi trả về
            try
            {
                JObject jsonObj = JObject.Parse(rawJson);

                // Mang Tên và Mô tả đi dịch lén qua Google Translate
                if (jsonObj["TenSP"] != null)
                    jsonObj["TenSP"] = DichSangTiengViet(jsonObj["TenSP"].ToString());

                if (jsonObj["MoTa"] != null)
                    jsonObj["MoTa"] = DichSangTiengViet(jsonObj["MoTa"].ToString());

                return jsonObj.ToString(Formatting.None);
            }
            catch
            {
                return rawJson; // Nếu lỗi dịch thì trả về tiếng Anh gốc
            }
        }

        // ==========================================
        // TOOL HACK: DỊCH TIẾNG ANH SANG TIẾNG VIỆT QUA GOOGLE FREE
        // ==========================================
        private static string DichSangTiengViet(string text)
        {
            try
            {
                // URL API ẩn của Google Translate (Dùng chùa không cần key)
                string url = String.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=vi&dt=t&q={0}", HttpUtility.UrlEncode(text));

                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = System.Text.Encoding.UTF8;
                    string result = webClient.DownloadString(url);

                    // Bóc tách mảng JSON lằng nhằng của Google để lấy đúng chữ tiếng Việt
                    JArray jsonArray = JArray.Parse(result);
                    string translatedText = "";
                    foreach (JArray item in jsonArray[0])
                    {
                        translatedText += Convert.ToString(item[0]);
                    }
                    return translatedText;
                }
            }
            catch
            {
                return text;
            }
        }

        // ==========================================
        // 2. GIÁM ĐỐC CHIẾN LƯỢC (OLLAMA LOCAL)
        // ==========================================
        public static string PhanTichDoanhThu(string duLieuDoanhThu)
        {
            // ĐÃ SỬA: Thay toán tử ?. và ?? bằng lệnh if-else truyền thống cho VS 2013 đọc được
            string currentTheme = HttpContext.Current.Application["CurrentTheme"] != null ? HttpContext.Current.Application["CurrentTheme"].ToString() : "banthuong";
            string mua = (currentTheme == "giangsinh" || currentTheme == "le304") ? "MÙA LỄ HỘI" : "NGÀY THƯỜNG";
            int phanTramGoiY = (currentTheme == "giangsinh" || currentTheme == "le304") ? 45 : 15;

            string heThongChiDao = @"You MUST output ONLY the following JSON object exactly:
            {
                ""NhanXet"": ""Doanh thu tháng này đang là " + duLieuDoanhThu + @". Em nghĩ nay đang là " + mua + @" nên sếp hãy giảm " + phanTramGoiY + @"% để kích cầu nhé!"",
                ""PhanTramGiam"": " + phanTramGoiY + @"
            }";

            var payload = new
            {
                model = "llava-phi3",
                prompt = "Output the JSON exactly as instructed.",
                system = heThongChiDao,
                stream = false
            };

            return GoiOllama(payload);
        }

        // ==========================================
        // 3. ĐỘNG CƠ CỐT LÕI OLLAMA
        // ==========================================
        private static string GoiOllama(object payload)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = Int32.MaxValue;
                string jsonPayload = js.Serialize(payload);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrlOllama);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 120000;

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream())) { sw.Write(jsonPayload); }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    string result = sr.ReadToEnd();
                    dynamic jsonResponse = js.Deserialize<dynamic>(result);
                    string rawJson = jsonResponse["response"].ToString();

                    rawJson = rawJson.Replace("```json", "").Replace("```", "").Trim();
                    int startIndex = rawJson.IndexOf('{');
                    int endIndex = rawJson.LastIndexOf('}');
                    if (startIndex != -1 && endIndex != -1 && endIndex > startIndex) { rawJson = rawJson.Substring(startIndex, endIndex - startIndex + 1); }
                    return rawJson;
                }
            }
            catch (Exception ex) { return "{\"Loi\": \"Lỗi kết nối Ollama: " + ex.Message + "\"}"; }
        }
    }
}