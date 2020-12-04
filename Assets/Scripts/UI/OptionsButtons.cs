using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

using DEEP.HUD;
using DEEP.Weapons;
using DEEP.Entities.Player;

namespace DEEP.UI { 

    public class OptionsButtons : MonoBehaviour {

        [SerializeField] Toggle fullscreen = null;//fullscreen toggle
        [SerializeField] Dropdown quality = null;//dropdown quality
        [SerializeField] Dropdown resolutionDropdown = null;//Dropdown resolution
        Resolution[] resolution;//vetor de resol.

        [SerializeField] Slider volume = null;//volume slider
        [SerializeField] TMP_InputField volumeIF = null;//volume input field

        [SerializeField] Slider mouseSensitivity = null;//mouse sensitivity slider
        [SerializeField] TMP_InputField mouseSensitivityIF = null;//mouse sensitivity input field

        [SerializeField] Slider fov = null;//fov slider
        [SerializeField] TMP_InputField fovIF = null;//fov input field

        [SerializeField] Toggle rightSideWeapon = null;// Weapon on the right side of the screen toggle
        [SerializeField] Toggle speedrun = null;// Speedrun HUD toggle
        [SerializeField] Toggle statistics = null;// Statistics hUD toggle

        protected void Awake() {
            Initialize();
        }

        // Initializes the options, should be called by MenuButtons at Start().
        // If not called options will only apply when entering the menu.
        public void Initialize() {

            resolution = Screen.resolutions;//get the possible resolutions 
            resolutionDropdown.ClearOptions();//clear the dropdown

            List<string> options = new List<string>();//cria uma lista de strings para o dropdown

            int currentindex = 0;
            for (int i = 0; i < resolution.Length; i++)
            {
                string option = resolution[i].width + " X " + resolution[i].height + " (" + resolution[i].refreshRate + "Hz)";// string no formato [widht X height (refreshRate)]
                options.Add(option);//adiciona a nova opcao na string

                if (resolution[i].height == Screen.height && resolution[i].width == Screen.width)
                    currentindex = i;//indica qual e o index da resolucao atual
                
            }

            resolutionDropdown.AddOptions(options);//adiciona as novas opcoes
            resolutionDropdown.value = currentindex;//adiciona a selecao atual
            resolutionDropdown.RefreshShownValue();//atualiza

            quality.value = QualitySettings.GetQualityLevel();
            fullscreen.isOn = Screen.fullScreen;   

            //pega o valor inicial do volume
            if(!PlayerPrefs.HasKey("Volume"))
                PlayerPrefs.SetFloat("Volume", 1.0f);
            volume.value = PlayerPrefs.GetFloat("Volume");

            //pega o valor inicial da sensibilidade do mouse
            if(!PlayerPrefs.HasKey("Mouse sensitivity"))
                PlayerPrefs.SetFloat("Mouse sensitivity", 7.0f);
            mouseSensitivity.value = PlayerPrefs.GetFloat("Mouse sensitivity");

            //pega o valor inicial do FOV
            if(!PlayerPrefs.HasKey("FoV"))
                PlayerPrefs.SetFloat("FoV", 75.0f);
            fov.value = PlayerPrefs.GetFloat("FoV");

            // Gets initial right side weapon value.
            if(!PlayerPrefs.HasKey("RightSideWeapon"))
                PlayerPrefs.SetInt("RightSideWeapon", 0);
            rightSideWeapon.isOn = (PlayerPrefs.GetInt("RightSideWeapon") != 0);

            // Gets initial speedrun HUD value.
            if(!PlayerPrefs.HasKey("SpeedrunHUD"))
                PlayerPrefs.SetInt("SpeedrunHUD", 0);
            speedrun.isOn = (PlayerPrefs.GetInt("SpeedrunHUD") != 0);

            // Gets initial statistics HUD value.
            if(!PlayerPrefs.HasKey("StatisticsHUD"))
                PlayerPrefs.SetInt("StatisticsHUD", 0);
            statistics.isOn = (PlayerPrefs.GetInt("StatisticsHUD") != 0);

        }

            //muda a resolucao pelo index selecionado
        public void SetResolution(int index)
        {
            Resolution newresolution = resolution[index];//seleciona com base no index
            Screen.SetResolution(newresolution.width, newresolution.height, Screen.fullScreen, newresolution.refreshRate); //aplica a nova resolucao
        }

        //entra e sai de FullScreen
        public void SetFullScreen(bool isOn)
        {
            Screen.fullScreen = isOn;
        }

        public void SetQuality(int qualityindex)//seleciona a qualidade de a cordo com o index atual aplicando ele no sistema base da unity
        {
            QualitySettings.SetQualityLevel(qualityindex);
        }

        // Changes the volume (through slider)
        public void SetVolume(float value)
        {
            // Applies setting
            PlayerPrefs.SetFloat("Volume", value);
            AudioListener.volume = value;

            // Updates the input field.
            string text = value.ToString("0.0");
            if(text.Length == 1)
                text += ".0";
            volumeIF.text = text;            
        }

        // Changes the volume (through input field)
        public void SetVolume(string str)
        {
            // Parses value
            float value = float.Parse(str);
            value = Mathf.Clamp(value, volume.minValue, volume.maxValue);

            // Applies setting
            PlayerPrefs.SetFloat("Volume", value);
            AudioListener.volume = value;

            // Updates the slider
            volume.value = value;

            // Updates the input field (used to correctly format values and ensure it's valid).
            string text = value.ToString("0.0");
            if(text.Length == 1)
                text += ".0";
            volumeIF.text = text; 
        }

        // Changes the mouse sensitivity (through slider)
        public void SetMouseSensitivity(float value)
        {
            // Applies setting
            PlayerPrefs.SetFloat("Mouse sensitivity", value);
            // Tries to update mouse sentitivity on player's movementation script.
            PlayerMovementation movementation = FindObjectOfType<PlayerMovementation>();
            if(movementation != null)
                movementation.SetMouseSensitivity(PlayerPrefs.GetFloat("Mouse sensitivity"));

            // Updates the input field.
            string text = value.ToString("0.0");
            if(text.Length == 1)
                text += ".0";
            mouseSensitivityIF.text = text;
        }

        // Changes the mouse sensitivity (through input field)
        public void SetMouseSensitivity(string str)
        {
            // Parses value
            float value = float.Parse(str);
            value = Mathf.Clamp(value, mouseSensitivity.minValue, mouseSensitivity.maxValue);

            // Applies setting
            PlayerPrefs.SetFloat("Mouse sensitivity", value);
            // Tries to update mouse sentitivity on player's movementation script.
            PlayerMovementation movementation = FindObjectOfType<PlayerMovementation>();
            if(movementation != null)
                movementation.SetMouseSensitivity(PlayerPrefs.GetFloat("Mouse sensitivity"));

            // Updates the slider
            mouseSensitivity.value = value;

            // Updates the input field (used to correctly format values and ensure it's valid).
            string text = value.ToString("0.0");
            if(text.Length == 1)
                text += ".0";
            mouseSensitivityIF.text = text; 
        }

        // Changes the FoV (through slider)
        public void SetFoV(float value)
        {
            // Applies setting
            PlayerPrefs.SetFloat("FoV", value);
            // Tries to update FoV on player controller's script.
            PlayerController controller = FindObjectOfType<PlayerController>();
            if(controller != null)
                controller.SetFoV(PlayerPrefs.GetFloat("FoV"));

            // Updates the input field.
            string text = value.ToString("0.0");
            if(text.Length == 1)
                text += ".0";
            fovIF.text = text;

            PlayerPrefs.SetFloat("FoV", fov.value);          
        }

        // Changes the FoV (through input field)
        public void SetFoV(string str)
        {
            // Parses value
            float value = float.Parse(str);
            value = Mathf.Clamp(value, fov.minValue, fov.maxValue);

            // Applies setting
            PlayerPrefs.SetFloat("FoV", value);
            // Tries to update FoV on player controller's script.
            PlayerController controller = FindObjectOfType<PlayerController>();
            if(controller != null)
                controller.SetFoV(PlayerPrefs.GetFloat("FoV"));

            // Updates the slider
            fov.value = value;

            // Updates the input field (used to correctly format values and ensure it's valid).
            string text = value.ToString("0.0");
            if(text.Length == 1)
                text += ".0";
            fovIF.text = text; 
        }

        // Enables or disables weapon on the right side.
        public void SetRightSideWeapon(bool isOn)
        {
            PlayerPrefs.SetInt("RightSideWeapon", isOn ? 1 : 0);
            // Tries to update weapon position.
            PlayerWeaponController weaponController = FindObjectOfType<PlayerWeaponController>();
            if(weaponController != null)
                weaponController.SetRightSideWeapon(isOn);
                
        }

        // Enables or disables the speedrun HUD.
        public void SetSpeedrunHUD(bool isOn)
        {
            PlayerPrefs.SetInt("SpeedrunHUD", isOn ? 1 : 0);
            // Tries to update speedrun HUD status.
            HUDController hud = FindObjectOfType<HUDController>();
            if(hud != null)
                hud.Speedrun.SetEnabled(isOn);
        }

        // Enables or disables the statistics HUD.
        public void SetStatisticsHUD(bool isOn)
        {
            PlayerPrefs.SetInt("StatisticsHUD", isOn ? 1 : 0);
            // Tries to update statistics HUD status.
            HUDController hud = FindObjectOfType<HUDController>();
            if(hud != null)
                hud.Statistics.SetEnabled(isOn);
        }

        // Resets audio config to the defaults.
        public void VideoDefaults() {

        }

        // Resets audio config to the defaults.
        public void AudioDefaults() {

            // Variables:
            float defaultVolume = 1.0f;

            // Does the setting.
            volume.value = defaultVolume;
            SetVolume(defaultVolume);

        }

        // Resets audio config to the defaults.
        public void GameDefaults() {

            // Variables:
            float defaultSensitivity = 7.0f;
            float defaultFoV = 75.0f;
            bool defaultRightSide = false;
            bool defaultSpeedrun = false;
            bool defaultStats = false;

            // Does the setting.
            mouseSensitivity.value = defaultSensitivity;
            SetMouseSensitivity(defaultSensitivity);

            fov.value = defaultFoV;
            SetFoV(defaultFoV);

            rightSideWeapon.isOn = defaultRightSide;
            SetRightSideWeapon(defaultRightSide);

            speedrun.isOn = defaultSpeedrun;
            SetSpeedrunHUD(defaultSpeedrun);

            statistics.isOn = defaultStats;
            SetStatisticsHUD(defaultStats);

        }

    }
}
