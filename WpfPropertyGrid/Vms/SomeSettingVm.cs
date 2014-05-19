namespace WpfPropertyGrid.Vms
{
    using System;

    public class SomeSettingVm : SettingsViewModelBase<SomeSetting>
    {
        public SomeSettingVm(IRepository repository, SomeSetting setting) 
            : base(repository, setting, (r,s)=>r.SaveSomeSettingAsync(s))
        {
        }
    }
}



