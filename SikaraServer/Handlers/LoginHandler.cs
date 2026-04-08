

using UtpTypes.Handlers;
using UtpTypes.Actions;
using UtpTypes.UtpMessageContext;

namespace SilkaraServer.Handlers;

public class LoginHandler : UtpHandlerBase
{
    public override short ActionCode => (short)ClientCode.Login;

    public override async Task HandleAsync(UtpContext ctx)
    {
        // var payload = (LoginPayload)ctx.Payload;

        // // 1. Проверяем данные (БД, кэш и т.д.)
        // var user = await CheckUserInDatabase(payload.Username, payload.Password);

        // if (user != null)
        // {
        //     // 2. Достаем менеджер состояний
        //     var stateManager = ctx.ServiceLocator.GetRequiredService<IStateService>();

        //     // 3. МЕНЯЕМ СОСТОЯНИЕ НА СЕРВЕРЕ
        //     // Теперь этот клиент официально "Authorized"
        //     stateManager.MoveTo(new AuthorizedState(user.Id, user.Role));

        //     // 4. Отвечаем клиенту успехом
        //     await ReplySuccess(ctx);
        // }
        // else
        // {
        //     await ReplyError(ctx, "Invalid credentials");
        // }

        throw new NotImplementedException();
    }
}