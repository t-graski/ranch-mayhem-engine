namespace ranch_mayhem_engine.UI.Helper;

public class BoxBuilder
{
   private readonly UiComponentOptions _componentOptions;
   private readonly string _id;
   private UiComponent _parent;

   public BoxBuilder(string id)
   {
      _id = id;
      _componentOptions = new UiComponentOptions();
   }
}