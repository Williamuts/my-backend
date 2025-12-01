using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Diagnostics;
using Group4RecycleApp.Models;

namespace Group4RecycleApp.Services
{
    public class ApiService
    {
        HttpClient _httpClient;
        string _baseUrl = "http://10.0.2.2:5291/api/";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30) // 增加超时
            };
        }

        // ============================================================
        //  回收提交 (NEW - 核心功能)
        // ============================================================
        public async Task<bool> SubmitRecycleAsync(string description, string photoPath, double latitude, double longitude)
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Error: No Auth Token found");
                    return false;
                }

                // 使用 MultipartFormDataContent 上传文件
                using (var content = new MultipartFormDataContent())
                {
                    // 添加文本字段
                    content.Add(new StringContent(description), "description");
                    content.Add(new StringContent(latitude.ToString()), "latitude");
                    content.Add(new StringContent(longitude.ToString()), "longitude");

                    // 添加照片文件
                    if (!string.IsNullOrEmpty(photoPath) && File.Exists(photoPath))
                    {
                        var fileStream = File.OpenRead(photoPath);
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        content.Add(fileContent, "photo", Path.GetFileName(photoPath));
                    }
                    else
                    {
                        Debug.WriteLine("Error: Photo file not found");
                        return false;
                    }

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await _httpClient.PostAsync("incident/submit", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Recycle submit successful");
                        return true;
                    }
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Submit Error: {response.StatusCode} - {errorMsg}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SubmitRecycle Exception: {ex.Message}");
                return false;
            }
        }

        // ============================================================
        //  Admin Requests - 获取待审项目 (待处理的请求/报告)
        // ============================================================
        public async Task<List<AdminRequest>> GetAdminRequestsAsync()
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");
                if (!string.IsNullOrEmpty(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // 改为调用 incident 控制器获取待审的报告
                var response = await _httpClient.GetAsync("incident/pending");

                if (response.IsSuccessStatusCode)
                {
                    var list = await response.Content.ReadFromJsonAsync<List<AdminRequest>>();
                    return list ?? new List<AdminRequest>();
                }
                else
                {
                    Debug.WriteLine($"GetAdminRequests Error: {response.StatusCode}");
                    return new List<AdminRequest>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAdminRequests Exception: {ex.Message}");
                return new List<AdminRequest>();
            }
        }

        public async Task<bool> ApproveAdminRequestAsync(int requestId)
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");
                if (!string.IsNullOrEmpty(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsync($"incident/{requestId}/approve", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ApproveAdminRequest Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RejectAdminRequestAsync(int requestId)
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");
                if (!string.IsNullOrEmpty(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsync($"incident/{requestId}/reject", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RejectAdminRequest Exception: {ex.Message}");
                return false;
            }
        }

        // ============================================================
        //  Admin Stats
        // ============================================================
        public async Task<AdminStats> GetAdminStatsAsync()
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Error: No Auth Token found");
                    return new AdminStats();
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("admin/stats");
                if (response.IsSuccessStatusCode)
                {
                    var stats = await response.Content.ReadFromJsonAsync<AdminStats>();
                    return stats ?? new AdminStats();
                }
                else
                {
                    Debug.WriteLine($"GetAdminStats Error: {response.StatusCode}");
                    return new AdminStats();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAdminStats Exception: {ex.Message}");
                return new AdminStats();
            }
        }

        // ============================================================
        //  回收指南 CRUD
        // ============================================================
        public async Task<List<RecyclingGuide>> GetRecyclingGuidesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("RecyclingGuide");

                if (response.IsSuccessStatusCode)
                {
                    var guides = await response.Content.ReadFromJsonAsync<List<RecyclingGuide>>();
                    return guides ?? new List<RecyclingGuide>();
                }
                else
                {
                    Debug.WriteLine($"GetGuides Error: {response.StatusCode}");
                    return new List<RecyclingGuide>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetGuides Exception: {ex.Message}");
                return new List<RecyclingGuide>();
            }
        }

        public async Task<RecyclingGuide> GetGuideByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"RecyclingGuide/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RecyclingGuide>();
                }
                else
                {
                    Debug.WriteLine($"GetGuideById Error: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetGuideById Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateGuideAsync(RecyclingGuide guide)
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Error: No Auth Token found");
                    return false;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("RecyclingGuide", guide);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"CreateGuide Error: {response.StatusCode} - {errorMsg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateGuide Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateGuideAsync(int id, RecyclingGuide guide)
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Error: No Auth Token found");
                    return false;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync($"RecyclingGuide/{id}", guide);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"UpdateGuide Error: {response.StatusCode} - {errorMsg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateGuide Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteGuideAsync(int id)
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Error: No Auth Token found");
                    return false;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"RecyclingGuide/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteGuide Exception: {ex.Message}");
                return false;
            }
        }

        // ============================================================
        //  回收中心 (E3)
        // ============================================================
        public async Task<List<RecyclingCenter>> GetRecyclingCentersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("recycling/all");

                if (response.IsSuccessStatusCode)
                {
                    var centers = await response.Content.ReadFromJsonAsync<List<RecyclingCenter>>();
                    return centers ?? new List<RecyclingCenter>();
                }
                else
                {
                    Debug.WriteLine($"GetRecyclingCenters Error: {response.StatusCode}");
                    return new List<RecyclingCenter>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetRecyclingCenters Exception: {ex.Message}");
                return new List<RecyclingCenter>();
            }
        }

        // ============================================================
        //  回收历史记录
        // ============================================================
        public async Task<List<RecycleSession>> GetRecyclingHistoryAsync()
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");

                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Error: No Auth Token found");
                    return new List<RecycleSession>();
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("Incident/history");

                if (response.IsSuccessStatusCode)
                {
                    var incidents = await response.Content.ReadFromJsonAsync<List<IncidentHistoryDto>>();

                    if (incidents == null || incidents.Count == 0)
                    {
                        return new List<RecycleSession>();
                    }

                    var sessions = incidents.Select(inc => new RecycleSession
                    {
                        Date = inc.ReportedAt,
                        Title = inc.Description,
                        Status = inc.Status,
                        StatusColor = GetStatusColor(inc.Status),
                        PhotoImage = inc.PhotoUrl,
                        TotalItems = 1,
                        Method = "App Submission",
                        Time = inc.ReportedAt.ToString("hh:mm tt"),
                        Items = new List<RecycledItemDetail>()
                    }).ToList();

                    return sessions;
                }
                else
                {
                    Debug.WriteLine($"GetRecyclingHistory Error: {response.StatusCode}");
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error Details: {errorMsg}");
                    return new List<RecycleSession>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetRecyclingHistory Exception: {ex.Message}");
                return new List<RecycleSession>();
            }
        }

        // ============================================================
        //  1. 登入 (Login)
        // ============================================================
        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync("auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (result != null)
                    {
                        Preferences.Set("AuthToken", result.Token);
                        Preferences.Set("UserEmail", result.Email ?? "");
                        Preferences.Set("UserFullName", result.FullName ?? "");
                        Preferences.Set("UserAddress", result.Address ?? "");
                        Preferences.Set("UserPhone", result.PhoneNumber ?? "");
                        Preferences.Set("UserAvatar", result.AvatarUrl ?? "");
                        Preferences.Set("UserRole", result.Role ?? "User");
                    }

                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    string errorMsg = $"Server Error: {response.StatusCode} - {errorContent}";
                    Debug.WriteLine(errorMsg);
                    throw new Exception(errorMsg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Connection Failed: {ex.Message}");
                throw;
            }
        }

        // ============================================================
        //  2. 注册 (Register)
        // ============================================================
        public async Task<bool> RegisterAsync(string email, string password, string fullName)
        {
            try
            {
                var registerData = new
                {
                    Username = email,
                    Email = email,
                    Password = password,
                    FullName = fullName
                };

                var response = await _httpClient.PostAsJsonAsync("auth/register", registerData);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Register Failed: {errorMsg}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Register Exception: {ex.Message}");
                return false;
            }
        }

        // ============================================================
        //  3. 更新個人資料 (Update Profile)
        // ============================================================
        public async Task<bool> UpdateProfileAsync(string fullName, string phone, string address)
        {
            try
            {
                string token = Preferences.Get("AuthToken", "");

                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Error: No Auth Token found");
                    return false;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var updateData = new
                {
                    FullName = fullName,
                    PhoneNumber = phone,
                    Address = address
                };

                var response = await _httpClient.PutAsJsonAsync("profile", updateData);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Update Failed: {response.StatusCode} - {errorMsg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update Connection Failed: {ex.Message}");
                return false;
            }
        }

        // 🔹 辅助方法：根据状态返回颜色
        private string GetStatusColor(string status)
        {
            return status?.ToLower() switch
            {
                "approved" => "#00C569",
                "submitted" => "#FFA500",
                "rejected" => "#FF0000",
                _ => "#808080"
            };
        }
    }

    // ============================================================
    //  DTO Models
    // ============================================================
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
        public string Role { get; set; }
    }

    public class IncidentHistoryDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime ReportedAt { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class AdminStats
    {
        public int TotalUsers { get; set; }
        public int PendingApprovals { get; set; }
    }
}