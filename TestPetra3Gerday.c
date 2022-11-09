#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <errno.h>
#include <netinet/in.h>
#include <netinet/tcp.h>
#include <arpa/inet.h>
#include <inttypes.h>
#include <netdb.h>
#include <pthread.h>
#include <signal.h>
#include <fcntl.h>
#include <time.h>
#include <termios.h>
#include <string.h>
#include <time.h>

int hSocketEcoute;
int hSocketService;
pthread_t tid;

struct  CAPTEURS
        {
                 unsigned L1 : 1;
                 unsigned L2 : 1;
                 unsigned T  : 1;
                 unsigned S  : 1;
                 unsigned CS : 1;
                 unsigned AP : 1;
                 unsigned PP : 1;
                 unsigned DE : 1;
              } ;

union
    {
    struct CAPTEURS capt ;
    unsigned char byte ;
    } u_capt ;
struct  ACTUATEURS
        {
                 unsigned CP : 2; //Chariot
                 unsigned C1 : 1; //Convoyer 1
                 unsigned C2 : 1; //Convoyer 2
                 unsigned PV : 1; //Plunger Vacuum
                 unsigned PA : 1; //Plunger Activate
                 unsigned AA : 1; //Arbre
                 unsigned GA : 1; //Grapin sur Arbre
        } ;
union
    {
    struct ACTUATEURS act ;
    unsigned char byte ;
    } u_act ;
void* threadCaptor(void *);
int port = 6670;
char hostname[30];
int fd_petra_in,fd_petra_out;
char msgClient[50];
char msgServeur[50];
int ret;
int captor;
int timeCaptor;
int serverType;

int main()
{


	printf("Enter type of server : \n");
	printf("1 for localhost\n");
	printf("2 for PETRA\n");
	scanf("%d",&serverType);
	fflush(stdin);

	if(serverType == 1)
	{
		strcpy(hostname,"192.168.182.134");
	}
	else
	{
	strcpy(hostname,"10.59.40.64");
	}
			
	strcpy(msgServeur,"Salut c est le serveur\n");
	printf("Ip : %s", hostname);

	struct in_addr adresseIP;
	struct hostent * infosHost;
	struct sockaddr_in adresseSocket;

	if((hSocketEcoute = socket(AF_INET,SOCK_STREAM,0)) == -1)
		printf("Erreur socket\n");
	else
		printf("Creation de la socket : OK\n");
	if((infosHost = gethostbyname(hostname)) == 0)
		printf("Erreur gethostbyname\n");
	else
		printf("Acquisition infos host : OK\n");

	memcpy(&adresseIP,infosHost->h_addr, infosHost->h_length);
	printf("Adresse IP = %s \n",inet_ntoa(adresseIP));

	memset(&adresseSocket,0,sizeof(struct sockaddr_in));
	adresseSocket.sin_family = AF_INET; /*Domaine*/
	adresseSocket.sin_port = htons(port);/*Conversion numéro / format réseau*/
	memcpy(&adresseSocket.sin_addr, infosHost->h_addr,infosHost->h_length);

	if(bind(hSocketEcoute,(struct sockaddr*)&adresseSocket,sizeof(struct sockaddr_in)) == -1)
		printf("Erreur bind\n");

	int tailleSockaddr_in;

	if(listen(hSocketEcoute,SOMAXCONN) == -1)
		printf("Erreur listen\n");

	tailleSockaddr_in = sizeof(struct sockaddr_in);

	if((hSocketService = accept(hSocketEcoute,(struct sockaddr *)&adresseIP,&tailleSockaddr_in)) == -1)
		printf("Erreur accept\n");

    printf("Serveur cree avec succes\n");
    u_act.byte = 0x00 ;
    u_act.act.PV = 1 ;


    if(serverType == 2)
    {
	    fd_petra_out = open ( "/dev/actuateursPETRA", O_WRONLY );
	    if ( fd_petra_out == -1 )
	    {
	        perror ( "MAIN : Erreur ouverture PETRA_OUT" );
	        return 1;
	    }
	    else
	        printf ("MAIN: PETRA_OUT opened\n");
	    fd_petra_in = open( "/dev/capteursPETRA",O_RDONLY);
		if (fd_petra_in == -1)
		{
			perror ("MAIN : Erreur ouverture PETRA_IN");
		}
		else
			printf ("MAIN: PETRA_IN opened\n");    	
    }



    u_act.byte = 0x00;
    printf("Serveur <: Creation thread capteur\n");
    pthread_create(&tid,NULL,threadCaptor,NULL);
    if(serverType == 2)
    	write ( fd_petra_out , &u_act.byte ,1 );
    while(1)
    {
        if((ret = recv(hSocketService,msgClient,50,0)) == -1)
    		printf("Error\n");
    	//msg = "2-5"
	    char * token = strtok(msgClient,"-");
	    captor = atoi(token);
	    token = strtok(NULL,"-");
	    timeCaptor = atoi(token);
	    printf("Serveur <: Activation Actuateur %d pour %d secondes\n",captor,timeCaptor);
	    switch(captor)
	    {
	    	case 1:{
	    		if(serverType == 2)
	    		{
	                u_act.act.C1 = 1;
	                write ( fd_petra_out , &u_act.byte ,1 );
	                sleep(timeCaptor);
	                u_act.act.C1 = 0;
	                write ( fd_petra_out , &u_act.byte ,1 );
	    		}
	    		else
	    		{
	    			printf("Serveur <: Simulation activation capteur C1 pendant %d secondes \n",timeCaptor);
	    		}

	    	}
	    	break;
	    	case 2:{
	    		if(serverType == 2)
	    		{
	                u_act.act.C2 = 1;
	                write ( fd_petra_out , &u_act.byte ,1 );
	                sleep(timeCaptor);
	                u_act.act.C2 = 0;
	                write ( fd_petra_out , &u_act.byte ,1 );	    			
	    		}
	    		else
	    		{
	    			printf("Serveur <: Simulation activation capteur C2 pendant %d secondes \n",timeCaptor);
	    		}

	    	}
	    	break;
	    	case 3:{
	    		if(serverType == 2)
	    		{
	                u_act.act.PV = 1;
	                write ( fd_petra_out , &u_act.byte ,1 );
	                sleep(timeCaptor);
	                u_act.act.PV = 0;
	                write ( fd_petra_out , &u_act.byte ,1 );
	    		}
	    		else
	    		{
	    			printf("Serveur <: Simulation activation capteur PV pendant %d secondes \n",timeCaptor);
	    		}

	    	}
	    	break;
	    	case 4:{
	    		if(serverType == 2)
	    		{
	                u_act.act.PA = 1;
	                write ( fd_petra_out , &u_act.byte ,1 );
	                sleep(timeCaptor);
	                u_act.act.PA = 0;
	                write ( fd_petra_out , &u_act.byte ,1 );
	    		}
	    		else
	    		{
	    			printf("Serveur <: Simulation activation capteur PA pendant %d secondes \n",timeCaptor);
	    		}

	    	}
	    	break;
	    	case 5:{
	    		if(serverType == 2)
	    		{
	                u_act.act.AA = 1;
	                write ( fd_petra_out , &u_act.byte ,1 );
	                sleep(timeCaptor);
	                u_act.act.AA = 0;
	                write ( fd_petra_out , &u_act.byte ,1 );
	    		}
	    		else
	    		{
	    			printf("Serveur <: Simulation activation capteur AA pendant %d secondes \n",timeCaptor);
	    		}

	    	}
	    	break;
	    	case 6:{
	    		if(serverType == 2)
	    		{
	                u_act.act.GA = 1;
	                write ( fd_petra_out , &u_act.byte ,1 );
	                sleep(timeCaptor);
	                u_act.act.GA = 0;
	                write ( fd_petra_out , &u_act.byte ,1 );
	    		}
	    		else
	    		{
	    			printf("Serveur <: Simulation activation capteur GA pendant %d secondes \n",timeCaptor);
	    		}

	    	}
	    	break;
	    	case 7:{
	    		if(serverType == 2)
	    		{
	                if (u_act.act.CP < 3)
	                    u_act.act.CP++;
	                else
	                    u_act.act.CP = 0;
		    		}
		    		else
	    		{
	    			printf("Serveur <: Simulation activation capteur CP pendant %d secondes \n",timeCaptor);
	    		}

	    	}
	    	break;
	    }
    }

}
void* threadCaptor(void *param)
{

	if(serverType == 2)
	{
		while(1)
		{
			read ( fd_petra_in , &u_capt.byte , 1 );

			msgServeur[0] = u_capt.capt.DE + '0';
			msgServeur[1] = u_capt.capt.CS + '0';
			msgServeur[2] = u_capt.capt.PP + '0';
			msgServeur[3] = u_capt.capt.S + '0';
			msgServeur[4] = u_capt.capt.L1 + '0';
			msgServeur[5] = u_capt.capt.L2 + '0';
			msgServeur[6] = u_capt.capt.AP + '0';
			printf("Serveur <: Envoie etat capteur\n");
			send(hSocketService,msgServeur,50,0);
			sleep(1);
		}		
	}
	else
	{
		srand(time(NULL));
		unsigned tab[5][7];
		int cpt = 0;
		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				tab[i][j] = rand()%2+1;
			}
		}
		while(1)
		{
			if(cpt > 4)
				cpt = 0;
			msgServeur[0] = tab[cpt][0] + '0';
			msgServeur[1] = tab[cpt][1] + '0';
			msgServeur[2] = tab[cpt][2] + '0';
			msgServeur[3] = tab[cpt][3] + '0';
			msgServeur[4] = tab[cpt][4] + '0';
			msgServeur[5] = tab[cpt][5] + '0';
			msgServeur[6] = tab[cpt][6] + '0';
			printf("Serveur <: Envoie etat capteur %s\n",msgServeur);
			send(hSocketService,msgServeur,50,0);
			cpt++;
			sleep(1);
		}
	}
}
