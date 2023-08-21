namespace PalettePlus.Interface.Windows.Tabs; 

public interface IWindowTab {
	public string Name { get; }

	public void Draw();
}