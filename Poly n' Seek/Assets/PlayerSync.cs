using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Cameras;
using UnityEngine.UI;

public class PlayerSync : NetworkBehaviour
{

    [Header("Hunter")]
    [Space(10)]
    public GameObject Hunt;
    public GameObject CamHunt;
    public GameObject RemoveGlasses;
    public FirstPersonController controllerHunter;
    public float Timer, EnviarCk;
    public GameObject Bullet, ArmaPosFire;
   
    
    [Header("Prop")]
    [Space(10)]
    public GameObject Prop;
    public GameObject CamProp;
    public GameObject CamPropObj;
    public ThirdPersonCharacter controllerProp;
    public ThirdPersonUserControl controllerProp2;
    public GameObject[] ObjetosMorph; // aqui define o ID do morph, sendo o primeiro como 0, ou seja se tiver 5 vao ter os id's (0,1,2,3,4) pois o 0 se torna um numero a se contar...
    public int IdMorph = 0;
    public bool Morreu;
    public GameObject ParticleDeath;
    public GameObject CanvasProp;
    public float CoolDown;
    public GameObject CanvasPronto,CanvasCoolDown;

    [Header("Audiosystem")]
    public AudioSource AudioHunt;
    public AudioClip FootStepsound;
    [Header("Config")]
    public GameObject TextTime;
    public GameObject Spectator,SpecCam;
    public GameObject TextoDerrotaHunt;
    public GameObject TextoVitoriaHunt;
    public GameObject TextoVitoriaProp;
    public bool AcabouOJogo;
    public GameObject CanvasPlayersConnected;
    public LayerMask Layers;
    public float DistanciaDoRaio;


    [Header("SyncVars")]
    [SyncVar] public bool IsHunterSync;
    [SyncVar] public bool EstaMortoSync;//se morreu
    [SyncVar] public int ObjectMorphIdSync;
    [SyncVar] public float TimerServerSync;
    [SyncVar] public bool StartGameSync;
   // [SyncVar] public bool EstaAndando;
    [SyncVar] public bool EstaProntoSync;


    //private's
    private RaycastHit Point;
    private bool Esperar, ApertouP;


    void Start()
    {
        Esperar = false;
        TextTime = GameObject.FindGameObjectWithTag("TempoText");


        if (!isLocalPlayer)
        {
            AudioHunt.clip = FootStepsound;
            AudioHunt.loop = true;

            //hunter
            CamHunt.GetComponent<Camera>().enabled = false;
            CamHunt.GetComponent<AudioListener>().enabled = false;
            CamHunt.tag = "Untagged";
            controllerHunter.enabled = false;

            //prop
            Prop.GetComponent<Move>().enabled = false;
            CamProp.GetComponent<Camera>().enabled = false;
            CamProp.GetComponent<AudioListener>().enabled = false;
            CamProp.tag = "Untagged";
            controllerProp.enabled = false;
            CanvasProp.SetActive(false);
            controllerProp2.enabled = false;
            

            //spec
            SpecCam.GetComponent<Camera>().enabled = false;
            SpecCam.GetComponent<AudioListener>().enabled = false;
            SpecCam.tag = "Untagged";
            Spectator.GetComponent<FirstPersonController>().enabled = false;
        }
       
        else
        { // se for o player
            RemoveGlasses.SetActive(false);
            CanvasPlayersConnected.SetActive(true);
            if (isServer)
            {
                CmdEnviarIshunter(isServer);//true
                CmdEnviarPronto(true);
                Timer = 50; // tempo maximo de espera menos 10
            }
            if (!isServer)//false
                CmdEnviarIshunter(isServer);//false
               
            


            if (isServer)
            {
                Hunt.SetActive(true);
                Hunt.transform.position = GameObject.FindGameObjectWithTag("Room").transform.position;
                controllerHunter.enabled = true;
            }
            if (!isServer)//false
            {
                Prop.SetActive(true);
                CamPropObj.SetActive(true);
                CamPropObj.GetComponent<FreeLookCam>().SetTarget(Prop.transform);
                controllerProp.enabled = true;
            }
        }
    }
    


    void Update()
    {
        CanvasPlayersConnected.GetComponent<Text>().text = GameObject.FindGameObjectsWithTag("Player").Length + " Jogadores Conectados";
        //all sync
        if(TimerServerSync > 0)
        {
            int timmm = (int)TimerServerSync - 10;
            if (timmm < 0 && StartGameSync == true)
            {
                timmm = 0;
            }
            if (!isServer)
            {
                if (StartGameSync == false)
                    TextTime.GetComponent<Text>().text = "Esconda-se Em:" + timmm + " Segundos";
                if (StartGameSync == true)
                    TextTime.GetComponent<Text>().text = "Fique Escondido Por:" + timmm + " Segundos";
            }
            if (isServer)
            {
                if (StartGameSync == false)
                    TextTime.GetComponent<Text>().text = "Espere Por:" + timmm + " Segundos";
                if (StartGameSync == true)
                    TextTime.GetComponent<Text>().text = "Procure-os:" + timmm + " Segundos";
            }
          
        }
     
        if (StartGameSync == true)
        {
            GameObject[] JogadoresAtuais = GameObject.FindGameObjectsWithTag("Player");
            int PropsTotal = 0;
            int PropsMortos = 0;
            for (int x = 0; x < JogadoresAtuais.Length; x++)
            {
                if (JogadoresAtuais[x].GetComponent<PlayerSync>().IsHunterSync == false)
                {
                    PropsTotal += 1;
                }
               
            }
            for (int x = 0; x < JogadoresAtuais.Length; x++)
            {
                if (JogadoresAtuais[x].GetComponent<PlayerSync>().EstaMortoSync == true)
                {
                    PropsMortos += 1;
                }
            }
       
          //  Debug.Log(PropsTotal + "total;   mortos >" + PropsMortos);
            if (PropsMortos == PropsTotal && isLocalPlayer)
            {
                AcabouOJogo = true;
                TextoVitoriaHunt.SetActive(true);
            }
        }


        
        if (StartGameSync == true)
        {
            GameObject[] JogadoresAtuais = GameObject.FindGameObjectsWithTag("Player");
            for (int x = 0; x < JogadoresAtuais.Length; x++)
            {
                JogadoresAtuais[x].GetComponent<PlayerSync>().CmdEnviarStart(StartGameSync);
            }
        }
        if (isLocalPlayer)
        {
            CamPropObj.GetComponent<FreeLookCam>().SetTarget(Prop.transform);
            GameObject[] JogadoresAtuais = GameObject.FindGameObjectsWithTag("Player");
            for (int x = 0; x < JogadoresAtuais.Length; x++)
            {
                if (JogadoresAtuais[x].GetComponent<PlayerSync>().IsHunterSync)
                {
                    float Temposo = JogadoresAtuais[x].GetComponent<PlayerSync>().TimerServerSync;
                    if (Temposo <= 7 && StartGameSync == true && isLocalPlayer)
                    {
                        TextoDerrotaHunt.SetActive(true);
                        TextoVitoriaProp.SetActive(true);
                        AcabouOJogo = true;
                    }
                }

            }
            if (isServer)
            {
                /*  if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                  {
                      CmdEnviarSomPassos(true);
                  }
                  else
                  {
                      CmdEnviarSomPassos(false);
                  }*/
                int JogadoresProntos = 0;
                for (int x = 0; x < JogadoresAtuais.Length; x++)
                {
                    if (JogadoresAtuais[x].GetComponent<PlayerSync>().EstaProntoSync == true)
                    {
                        JogadoresProntos += 1;
                    }
                }
                //  Debug.Log("Prontos:" + JogadoresProntos + "   Atuais:" + JogadoresAtuais.length);
                if (JogadoresProntos > 1)
                {
                    if (JogadoresProntos == JogadoresAtuais.Length)
                    {
                        Esperar = true;
                    }
                }
                if (Input.GetMouseButtonDown(0) && AcabouOJogo == false)
                {
                    GameObject Bala = Instantiate(Bullet, ArmaPosFire.transform.position, ArmaPosFire.transform.rotation, null);
                    NetworkServer.Spawn(Bala);
                }
                CmdEnviarIshunter(isServer);//true
                //tiemr system
                if (Esperar)
                    if (Timer <= 10 && StartGameSync == false)
                    {
                        CmdEnviarStart(true);
                        Timer = 310; // tempo maximo de procura menos 10, ex se quiser 300 coloque 310
                        Hunt.transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
                    }
                if (Esperar)
                    if (StartGameSync == false)
                    {
                        if (JogadoresAtuais.Length > 1)
                        {

                            if (AcabouOJogo == false)
                                Timer -= 1 * Time.deltaTime;
                            EnviarCk += 1 * Time.deltaTime;
                            if (EnviarCk >= 1)
                            {
                                EnviarCk = 0;
                                CmdEnviarTimer(Timer);
                            }
                        }
                    }
                if (Esperar)
                    if (StartGameSync == true)
                    {

                        if (AcabouOJogo == false)
                            Timer -= 1 * Time.deltaTime;
                        EnviarCk += 1 * Time.deltaTime;
                        if (EnviarCk >= 1)
                        {
                            CmdEnviarStart(true);
                            EnviarCk = 0;
                            CmdEnviarTimer(Timer);
                        }
                    }//tiemr system end

                // CmdEnviarSomPassos(Batendo);
            }


            if (!isServer)
            {//false
                if (IsHunterSync)
                {
                    /*  if (EstaAndando)
                      {
                          AudioHunt.Play();
                      }
                      else
                      {
                          AudioHunt.Stop();
                      }*/
                }
                CmdEnviarIshunter(isServer);//false
                if (Input.GetKeyDown(KeyCode.P))
                {
                    CmdEnviarPronto(true);
                    ApertouP = true;
                    CanvasPronto.SetActive(false);
                }

                if (Morreu == true && AcabouOJogo == false)
                {
                    EnviarCk += 1 * Time.deltaTime;
                    if (EnviarCk >= 0.3f)
                    {
                        EnviarCk = 0;
                        CmdEnviarMorto(Morreu);
                    }
                }
            }
            CmdEnviarMorph(IdMorph);





            if (IsHunterSync == false) // aqui é do jogador local, o player atual
            {

                if (Morreu && Prop.activeInHierarchy == true && AcabouOJogo == false)
                {
                    Prop.SetActive(false);
                    CamPropObj.SetActive(false);
                    Spectator.SetActive(true);
                }
                //prop morph system
                int jogadoresProntos = 0;
                for (int p = 0; p < JogadoresAtuais.Length; p++)
                {
                    if (JogadoresAtuais[p].GetComponent<PlayerSync>().EstaProntoSync == true)
                    {
                        jogadoresProntos += 1;
                    }
                }
                if (ApertouP && jogadoresProntos == JogadoresAtuais.Length)
                {
                    if (Physics.Raycast(CamProp.transform.position, CamProp.transform.forward, out Point, DistanciaDoRaio, Layers, QueryTriggerInteraction.Ignore))
                    {
                        Debug.DrawLine(CamProp.transform.position, Point.point, Color.green);
                        if (Point.transform.gameObject.tag == "ObjetoProp")
                        {
                            if (Input.GetKeyDown(KeyCode.E))
                                IdMorph = Point.transform.gameObject.GetComponent<IdMorph>().Id;
                        }
                    }

                    if (CoolDown < 20)
                    {
                        CoolDown += 1 * Time.deltaTime;
                        CanvasCoolDown.GetComponent<Text>().text = "Espere Por: " + (int)CoolDown + " Para Usar a Habilidade";
                    }
                    else
                    {
                        CanvasCoolDown.GetComponent<Text>().text = "Pressione Q Para Ser Um Objeto Aleatrorio";
                    }
                    if (Input.GetKeyDown(KeyCode.Q) &&  CoolDown >= 20)
                    {
                        int R = Random.Range(0, ObjetosMorph.Length);
                        if (R == IdMorph)
                        {
                            if (IdMorph != 0)
                                R = 0;
                            if (IdMorph == 0)
                                R = ObjetosMorph.Length;
                        }
                        IdMorph = R;
                        CoolDown = 0;
                    }
                }
                /*                           caso queira que se transforme a partir do Q e E para testar... so remover o "*/                       /*"
                    if (Input.GetKeyDown(KeyCode.E))
                {
                    IdMorph += 1;
                    if (IdMorph > ObjetosMorph.Length - 1)
                        IdMorph = 0;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    IdMorph -= 1;
                    if (IdMorph <= -1)
                        IdMorph = ObjetosMorph.Length - 1;
                }*/

                for (int x = 0; x < ObjetosMorph.Length; x++)
                {
                    ObjetosMorph[ObjectMorphIdSync].SetActive(true);
                    if (x != ObjectMorphIdSync)
                        ObjetosMorph[x].SetActive(false);
                }//prop morph end
            }
        }








        if (!isLocalPlayer) // not local not local not local not local not local not local not local not local
        {//aqui é o jogador que é visto por outros

            if (EstaMortoSync && Prop.activeInHierarchy == true)
            {
                Hunt.SetActive(false);
                Prop.SetActive(false);
                ParticleDeath.SetActive(true);
                ParticleDeath.transform.position = Prop.transform.position;
                Destroy(ParticleDeath, 8);
            }
            if (IsHunterSync)
            {
                Hunt.SetActive(true);
            }
            if (!IsHunterSync && !EstaMortoSync)
            {
                Prop.SetActive(true);
                for (int x = 0; x < ObjetosMorph.Length; x++)
                {
                    ObjetosMorph[ObjectMorphIdSync].SetActive(true);
                    if(x != ObjectMorphIdSync)
                    ObjetosMorph[x].SetActive(false);
                }
            }
        }
    }








    #region  Syncvar Eventos

    //sync Morto
    [Command]
    void CmdEnviarMorto(bool Gatilho)
    {
        this.EstaMortoSync = Gatilho;
        RpcReceberMorto(Gatilho);
    }
    [ClientRpc]
    void RpcReceberMorto(bool Gatilho)
    {
        this.EstaMortoSync = Gatilho;
    }  
    //sync Pronto
    [Command]
    void CmdEnviarPronto(bool Gatilho)
    {
        this.EstaProntoSync = Gatilho;
        RpcReceberPronto(Gatilho);
    }
    [ClientRpc]
    void RpcReceberPronto(bool Gatilho)
    {
        this.EstaProntoSync = Gatilho;
    }  

    //sync EstaAndando
   /* [Command]
    void CmdEnviarSomPassos(bool Gatilho)
    {
        this.EstaAndando = Gatilho;
        RpcReceberSomPassos(Gatilho);
    }
    [ClientRpc]
    void RpcReceberSomPassos(bool Gatilho)
    {
        this.EstaAndando = Gatilho;
    }*/

    //sync SatrtGame
    [Command]
    void CmdEnviarStart(bool Gatilho)
    {
        this.StartGameSync = Gatilho;
        RpcReceberStart(Gatilho);
    }
    [ClientRpc]
    void RpcReceberStart(bool Gatilho)
    {
        this.StartGameSync = Gatilho;
    }

    //sync TimerServer
    [Command]
    void CmdEnviarTimer(float Gatilho)
    {
        this.TimerServerSync = Gatilho;
        RpcReceberTimer(Gatilho);
    }
    [ClientRpc]
    void RpcReceberTimer(float Gatilho)
    {
        this.TimerServerSync = Gatilho;
    }

      //sync ObjectMorph
    [Command]
    void CmdEnviarMorph(int Gatilho)
    {
        this.ObjectMorphIdSync = Gatilho;
        RpcReceberMorph(Gatilho);
    }
    [ClientRpc]
    void RpcReceberMorph(int Gatilho)
    {
        this.ObjectMorphIdSync = Gatilho;
    }


    // Sync IsHunter
    [Command]
    void CmdEnviarIshunter(bool Gatilho)
    {
        this.IsHunterSync = Gatilho;
        RpcReceberIsHunter(Gatilho);
    }
    [ClientRpc]
    void RpcReceberIsHunter(bool Gatilho)
    {
        this.IsHunterSync = Gatilho;
    }
    #endregion // Syncvar's
}
