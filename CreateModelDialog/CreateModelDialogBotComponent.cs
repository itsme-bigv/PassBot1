using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CreateModelDialog.Actions;

namespace CreateModelDialog
{
    public class CreateModelDialogBotComponent : BotComponent
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Anything that could be done in Startup.ConfigureServices can be done here.
            // In this case, the CreateModelDialog needs to be added as a new DeclarativeType.
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<CreateModelDialog>(CreateModelDialog.Kind));
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<CreateMessageExchange>(CreateMessageExchange.Kind));
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<CreateSubject>(CreateSubject.Kind));
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<ExportModel>(ExportModel.Kind));
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<SubjectSuggestions>(SubjectSuggestions.Kind));
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<SubjectSuggestionComponent>(SubjectSuggestionComponent.Kind));
        }
    }
}
