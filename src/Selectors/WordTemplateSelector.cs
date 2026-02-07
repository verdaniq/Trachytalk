using Trachytalk.Models;

namespace Trachytalk.Selectors;

public class WordTemplateSelector : DataTemplateSelector
{
	public DataTemplate WordTemplate { get; set; }
	public DataTemplate CurrentWordTemplate { get; set; }

	protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
	{
		return ((Word)item).IsCurrentWord ? CurrentWordTemplate : WordTemplate;
	}
}

