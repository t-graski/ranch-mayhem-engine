namespace ranch_mayhem_engine.UI.Helper;

public class BoxBuilder : UiComponentBuilder<BoxBuilder>
{
    private readonly UiComponentOptions _componentOptions;

    public BoxBuilder(string id) : base(id, new UiComponentOptions())
    {
        _componentOptions = new UiComponentOptions();
    }
}