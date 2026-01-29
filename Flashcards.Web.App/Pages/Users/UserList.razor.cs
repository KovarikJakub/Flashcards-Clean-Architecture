using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages.Users;

public partial class UserList : PageBase
{
    [Inject]
    private UserFacade UserFacadeService { get; set; } = null!;

    private List<UserListModel>? Users { get; set; }

    private Dictionary<Guid, string?> UserPhotos { get; set; } = new();

    private bool IsCreating { get; set; }
    private bool IsSubmitting { get; set; }
    private string? ErrorMessage { get; set; }
    private NewUserModel NewUser { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await Load();
        await base.OnInitializedAsync();
    }

    private async Task Load()
    {
        Users = (await UserFacadeService.GetAllAsync()).ToList();
        _ = LoadUserPhotosAsync();
    }

    private async Task LoadUserPhotosAsync()
    {
        if (Users == null) return;

        foreach (var user in Users)
        {
            if (UserPhotos.ContainsKey(user.Id)) continue;

            try
            {
                var detail = await UserFacadeService.GetByIdAsync(user.Id);

                if (detail != null)
                {
                    UserPhotos[user.Id] = detail.PhotographyUrl;
                    StateHasChanged();
                }
            }
            catch
            {
                
            }
        }
    }

    private void ShowCreateForm()
    {
        IsCreating = true;
        ErrorMessage = null;
        NewUser = new NewUserModel
        {
            Role = RoleType.User
        };
    }

    private void CancelCreate()
    {
        IsCreating = false;
        ErrorMessage = null;
        NewUser = new NewUserModel();
    }

    private async Task CreateUser()
    {
        ErrorMessage = null;
        IsSubmitting = true;

        try
        {
            if (string.IsNullOrWhiteSpace(NewUser.UserName) ||
                string.IsNullOrWhiteSpace(NewUser.Password) ||
                string.IsNullOrWhiteSpace(NewUser.Email) ||
                string.IsNullOrWhiteSpace(NewUser.FullName))
            {
                ErrorMessage = "Please fill in all required fields.";
                return;
            }

            // TODO: create user with identity

            IsCreating = false;
            NewUser = new NewUserModel();
            await Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error creating user: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private async Task DeleteUser(UserListModel user)
    {
        if (user.Role == RoleType.Admin) return;

        if (user.Id != Guid.Empty)
        {
            try
            {
                // TODO: Delete user
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting user: {ex.Message}";
                return;
            }
        }
        Users?.Remove(user);

        if (UserPhotos.ContainsKey(user.Id))
            UserPhotos.Remove(user.Id);
    }

    private async Task SaveAll()
    {
        if (Users == null) return;
        ErrorMessage = null;

        try
        {
            foreach (var u in Users)
            {
                string? currentPhoto = UserPhotos.ContainsKey(u.Id) ? UserPhotos[u.Id] : null;

                var detail = new UserDetailModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    PhotographyUrl = currentPhoto,
                    Role = u.Role
                };

                await UserFacadeService.UpsertAsync(detail);
            }
            await Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving users: {ex.Message}";
        }
    }

    private class NewUserModel
    {
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? ProfilePicUrl { get; set; }
        public RoleType Role { get; set; }
    }
}