namespace MoreCyclopsUpgrades.Config;

using System.Collections.Generic;
using MoreCyclopsUpgrades.Config.Options;
using Nautilus.Handlers;
using Nautilus.Options;

internal class ModConfigMenuOptions : ModOptions
{
    private readonly IEnumerable<ConfigOption> configOptions;

    public ModConfigMenuOptions(IEnumerable<ConfigOption> options) : base("MoreCyclopsUpgrades Config Options")
    {
        configOptions = options;
    }

    public void RegisterEvents(ModConfig config)
    {
        SetUpEvents(config);
        OptionsPanelHandler.RegisterModOptions(this);
    }

    private void SetUpEvents(ModConfig config)
    {
        foreach (ConfigOption item in configOptions)
        {
            switch (item.OptionType)
            {
                case OptionTypes.Slider when item is SliderOption slider:

                    var sliderOption = ModSliderOption.Create(slider.Id, slider.Label, slider.MinValue, slider.MaxValue, slider.Value);
                    sliderOption.OnChanged += (object sender, SliderChangedEventArgs e) =>
                    {
                        if (e.Id == slider.Id)
                            slider?.ValueChanged(e.Value, config);
                    };
                    AddItem(sliderOption);
                    break;
                case OptionTypes.Choice when item is ChoiceOption choice:

                    var choiceOption = ModChoiceOption<string>.Create(choice.Id, choice.Label, choice.Choices, choice.Index);
                    choiceOption.OnChanged  += (object sender, ChoiceChangedEventArgs<string> e) =>
                    {
                        if (e.Id == choice.Id)
                            choice?.ChoiceChanged(e.Index, config);
                    };
                    AddItem(choiceOption);
                    break;
                case OptionTypes.Toggle when item is ToggleOption toggle:

                    var toggleOption = ModToggleOption.Create(toggle.Id, toggle.Label, toggle.State);
                    toggleOption.OnChanged += (object sender, ToggleChangedEventArgs e) =>
                    {
                        if (e.Id == toggle.Id)
                            toggle?.OptionToggled(e.Value, config);
                    };

                    AddItem(toggleOption);
                    break;
            }
        }
    }
}
