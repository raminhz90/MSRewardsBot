namespace MicrosoftRewardsBot.Contracts.ViewModels
{
    public interface INavigationAware
    {
        void OnNavigatedToAsync(object parameter);

        void OnNavigatedFrom();
    }
}
