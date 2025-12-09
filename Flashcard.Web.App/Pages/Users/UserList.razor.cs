using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages.Users;

public partial class UserList : PageBase
{
    [Inject]
    private UserFacade UserFacadeService { get; set; } = null!;

    private List<UserListModel>? Users { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await Load();
        await base.OnInitializedAsync();
    }

    private async Task Load()
    {
        Users = (await UserFacadeService.GetAllAsync()).ToList();
    }

    private void AddNewUser()
    {
        var user = new UserListModel { Id = Guid.NewGuid(), Name = "", Role = Flashcards.Common.Enums.RoleType.User };
        Users ??= new List<UserListModel>();
        Users.Insert(0, user);
    }

    private async Task DeleteUser(UserListModel user)
    {
        if (user.Id != Guid.Empty)
        {
            await UserFacadeService.DeleteAsync(user.Id);
        }
        Users.Remove(user);
    }

    private async Task SaveAll()
    {
        if (Users == null) return;

        foreach (var u in Users)
        {
            var detail = new UserDetailModel
            {
                Id = u.Id,
                Name = u.Name,
                PhotographyUrl = null,
                Role = u.Role
            };

            await UserFacadeService.UpsertAsync(detail);
        }

        await Load();
    }
}
