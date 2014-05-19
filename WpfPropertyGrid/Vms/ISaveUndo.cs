namespace WpfPropertyGrid.Vms
{
    using System.Windows.Input;

    public interface ISaveUndo
    {
        ICommand SaveCommand { get; }
        ICommand UndoCommand { get; }
        bool CanSave();
        void Save();
        bool CanUndo();
        void Undo();
    }
}