using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace DEEP.UI { 

    public class OptionsButtons : MonoBehaviour
    {

        public Slider mouseSensitivity;//mouse sensitivity slider
        public Toggle fullscreen;//fullscreen toggle
        private bool startscreen;//fullscreen or not in start
        public Dropdown quality;//dropdown quality
        public Dropdown resolutionDropdown;//Dropdown resolution
        Resolution[] resolution;//vetor de resol.
        public Slider volume;//volume sensitivity slider


        void Start()
        {
            
            //pega o valor inicial do volume
            if(!PlayerPrefs.HasKey("Mouse sensitivity"))
                PlayerPrefs.SetFloat("Mouse sensitivity", 6.0f);
            mouseSensitivity.value = PlayerPrefs.GetFloat("Mouse sensitivity");

            startscreen = Screen.fullScreen;
            resolution = Screen.resolutions;//get the possible resolutions 
            resolutionDropdown.ClearOptions();//clear the dropdown

            List<string> options = new List<string>();//cria uma lista de strings para o dropdown

            int currentindex = 0;
            for (int i = 0; i < resolution.Length; i++)
            {
                string option = resolution[i].width + " X " + resolution[i].height;// string no formato [widht X height]
                options.Add(option);//adiciona a nova opcao na string

                if (resolution[i].height == Screen.height && resolution[i].width == Screen.width)
                {
                    currentindex = i;//indica qual e o index da resolucao atual
                }
            }

            resolutionDropdown.AddOptions(options);//adiciona as novas opcoes
            resolutionDropdown.value = currentindex;//adiciona a selecao atual
            resolutionDropdown.RefreshShownValue();//atualiza

            quality.value = QualitySettings.GetQualityLevel();
            fullscreen.isOn = startscreen;   

            //pega o valor inicial do volume
            if(!PlayerPrefs.HasKey("Volume"))
                PlayerPrefs.SetFloat("Volume", 1.0f);
            volume.value = PlayerPrefs.GetFloat("Volume");

        }

        //muda a sensibilidade do mouse
        public void SetMouseSensitivity()
        {
            PlayerPrefs.SetFloat("Mouse sensitivity", mouseSensitivity.value);
        }

            //muda a resolucao pelo index selecionado
        public void SetResolution(int index)
        {
            Resolution newresolution = resolution[index];//seleciona com base no index
            Screen.SetResolution(newresolution.width, newresolution.height, Screen.fullScreen);//aplica a nova resolucao
        }

        //entra e sai de FullScreen
        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
        }

        public void SetQuality(int qualityindex)//seleciona a qualidade de a cordo com o index atual aplicando ele no sistema base da unity
        {
            QualitySettings.SetQualityLevel(qualityindex);
        }

        //muda o volume
        public void SetVolume()
        {
            PlayerPrefs.SetFloat("Volume", volume.value);
            AudioListener.volume = volume.value;
        }

    }
}
