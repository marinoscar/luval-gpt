using Luval.Framework.Core.Cache;
using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Luval.WebGPT.Presenter
{
    public class PresenterBase
    {

        protected virtual ILogger Logger { get; private set; }
        protected virtual IRepository Repository { get; private set; }
        protected virtual IHttpContextAccessor Context { get; private set; }

        protected virtual ICacheProvider<string, AppUser> UserCache { get; private set; }

        public PresenterBase(ILogger logger, IRepository repository, IHttpContextAccessor context, ICacheProvider<string, AppUser> userCache)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            UserCache = userCache ?? throw new ArgumentNullException(nameof(userCache));
        }

        protected virtual AppUser GetAppUser()
        {
            var webUser = Context.HttpContext.User.ToUser();

            var user = Repository.GetApplicationUser(webUser.ProviderName, webUser.ProviderKey);
            if (user == null)
                user = Repository.CreateAppUser(webUser.ToAppUser());
            return user;

        }
    }
}
