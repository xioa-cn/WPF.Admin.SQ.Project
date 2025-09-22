using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PressMachineMainModeules.Models
{
    public partial class InvoMaualModel : ObservableObject
    {
        [ObservableProperty] private float _手动速度;
        [ObservableProperty] private float _手动保护压力;
        [ObservableProperty] private float _设定频率;
        [ObservableProperty] private float _设定速度;
        [ObservableProperty] private float _每千克对应扭矩;
        [ObservableProperty] private float _压力浮动范围;
        [ObservableProperty] private float _设定正向压力;
        [ObservableProperty] private float _正向压力补偿;
        [ObservableProperty] private float _设定负向压力;
        [ObservableProperty] private float _负向压力补偿;


        public void Setself(InvoMaualModel dto)
        {
            this.手动速度 = dto.手动速度;
            this.手动保护压力 = dto.手动保护压力;
            this.设定频率 = dto.设定频率;
            this.设定速度 = dto.设定速度;
            this.每千克对应扭矩 = dto.每千克对应扭矩;
            this.压力浮动范围 = dto.压力浮动范围;
            this.设定正向压力 = dto.设定正向压力;
            this.正向压力补偿 = dto.正向压力补偿;
            this.设定负向压力 = dto.设定负向压力;
            this.负向压力补偿 = dto.负向压力补偿;
        }

        [RelayCommand]
        private void Add(string key)
        {
            switch (key)
            {
                case "正向压力补偿":
                    this.正向压力补偿++;
                    break;
                case "负向压力补偿":
                    this.负向压力补偿++;
                    break;
            }

        }

        [RelayCommand]
        private void Cutdown(string key)
        {
            switch(key)
            {
                case "正向压力补偿":
                    this.正向压力补偿--;
                    break;
                case "负向压力补偿":
                    this.负向压力补偿--;
                    break;
                }
            }
    }
}
