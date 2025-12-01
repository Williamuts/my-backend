using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Group4RecycleApp.Models;

namespace Group4RecycleApp.ViewModels
{
    public partial class FaqViewModel : ObservableObject
    {
        public ObservableCollection<FaqItem> Questions { get; } = new();

        public FaqViewModel()
        {
            LoadQuestions();
        }

        void LoadQuestions()
        {
            Questions.Add(new FaqItem
            {
                Question = "Why can’t I put the paper cup in the recycling bin?",
                Answer = "The plastic lining inside the cup cannot be separated from the paper during the standard pulping process, which contaminates the paper batch."
            });

            Questions.Add(new FaqItem
            {
                Question = "What if the cup says \"Compostable\"?",
                Answer = "Compostable cups require high heat in an industrial composting facility to break down. Do not put them in your home garden compost; check if your city offers \"Green Bin\" organics collection."
            });

            Questions.Add(new FaqItem
            {
                Question = "Can I put light bulbs in the glass recycling bin?",
                Answer = "No. Light bulbs use a different type of glass with a higher melting point than jars and bottles. Mixing them ruins the new glass being made."
            });

            Questions.Add(new FaqItem
            {
                Question = "What do I do if I break a CFL bulb?",
                Answer = "Do not vacuum immediately. Open windows to ventilate the room for 15 minutes, scoop up the debris with stiff paper, and wipe the area with a wet paper towel. Seal everything in a plastic bag before disposal."
            });

            Questions.Add(new FaqItem
            {
                Question = "Can I recycle plastic grocery bags?",
                Answer = "Not in your curbside bin. Bags get tangled in the sorting machines. Please take them to grocery stores that have specific \"Plastic Film/Bag\" drop-off bins."
            });

            Questions.Add(new FaqItem
            {
                Question = "Do I need to remove the label?",
                Answer = "No, the recycling process includes a heating stage where labels and glues are burned off or separated."
            });

            Questions.Add(new FaqItem
            {
                Question = "What happens if I recycle a dirty container?",
                Answer = "It can contaminate the whole bale of plastic, potentially causing the recycling center to send the entire load to the landfill. A quick rinse is essential!"
            });

            Questions.Add(new FaqItem
            {
                Question = "What should I do with dirty diapers?",
                Answer = "You should flush the excrement down the toilet and then place the diaper in the trash. Also, consider using reusable cloth diapers instead of disposable diapers."
            });

            Questions.Add(new FaqItem
            {
                Question = "What should I do with old clothes and old shoes?",
                Answer = "Gently-worn clothes and shoes can be donated to many charities. For damaged clothes and shoes, please double check with your local charity to see if it will accept them. Additionally, some retail stores recycle clothing or shoes."
            });

            Questions.Add(new FaqItem
            {
                Question = "What’s the best way to recycle (whole) glass?",
                Answer = "Check with your local program first when recycling (whole) glass. Most curbside community recycling programs accept different glass colors and types mixed together."
            });

            Questions.Add(new FaqItem
            {
                Question = "Can I put batteries in my recycling bin?",
                Answer = "NEVER. Tape the ends of the batteries and take them to a specialized battery drop-off point (often found at hardware or electronics stores)."
            });

            Questions.Add(new FaqItem
            {
                Question = "What do I do with old charging cables and headphones?",
                Answer = "Do not put them in the bin. Take them to an electronics (E-Waste) recycling center."
            });
        }



        [RelayCommand]
        void ToggleAnswer(FaqItem item)
        {

            item.IsExpanded = !item.IsExpanded;

        }
    }
}