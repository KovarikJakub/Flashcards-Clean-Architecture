using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Flashcards.Web.App.Pages.Statistics
{
    public partial class StatisticsPage : PageBase
    {
        // injections
        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
        [Inject]
        private UserFacade UserFacade { get; set; } = null!;
        [Inject]
        private LessonFacade LessonFacade { get; set; } = null!;
        
        // variables and properties
         private List<LessonListModel>? _lessons;
         private UserDetailModel? _user; 
         private bool _isLoading = true;
         private string? _userId;
        
        // computed properties
        private long TotalCardsSeen => _lessons?.Sum(l => l.Seen) ?? 0;
        private double GlobalAccuracy
        {
            get
            {
                if (_lessons == null || !_lessons.Any()) return 0;
                var totalCorrect = _lessons.Sum(l => (double)l.Correct);
                var totalWrong = _lessons.Sum(l => (double)l.Wrong);
                var totalAnswers = totalCorrect + totalWrong;
                return totalAnswers == 0 ? 0 : (totalCorrect / totalAnswers) * 100;
            }
        }
    
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // get current user identity
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    _userId = user.FindFirst("sub")?.Value;
                }
                
                await LoadData();                               // load lessons from logged user
                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadData()
        {
            if (string.IsNullOrEmpty(_userId))
            {
                // No user is  logged in
                _lessons = new List<LessonListModel>();
                Console.WriteLine("No user found");
            }
            else
            {
                // get current user
                _user = await UserFacade.GetByIdentityIdAsync(_userId);
                // get lessons for current user
                var query = new LessonQueryModel
                {
                    UserId = _user.Id
                };
                var lessonsPagedResult = await LessonFacade.GetByQueryAsync(query);
                _lessons = lessonsPagedResult.Items.ToList();
            }
        }
        
        // helper methods
        private static double CalculateAccuracy(double correct, double wrong)
        {
            var total = correct + wrong;
            if (total == 0) return 0;
            return (correct / total) * 100;
        }

        private static string GetAccuracyColor(double accuracy) => accuracy switch
        {
            >= 80 => "text-green-600",
            >= 50 => "text-yellow-600",
            _ => "text-red-600"
        };

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            return name.Trim().Substring(0, 1).ToUpper();
        }
    }
};

