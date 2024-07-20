N100 ;Nom du programme: SMSC531252312A1P01_001
N110 ;Version post pro: MASTERCAM M2021 réalisé par J.VALIERE
N120 ;Généré le 3/4/24 à 14:27
N4 ;Temps usinage:1min8s
N5 ;Dimensions du brut: X=117 Y=53 Z=6
N15 ;Type de programme: 
N25 ;Origine programme: 
N35 ;
N45 ;Liste des outils utilisés
N55 ; T5 FRAISE 2T D- 10
N65 ;
N75 ;Début du programme
N85 V.E.P185 = 1 ; =0 si Z0 sur matière, =1 si Z0 sur martyr
N95 V.E.P200 = 11  ; épaisseur matière en mm
N105 LSub_Prog_Start.nc
N115 ;
N125 ;Usinage avec FRAISE 2T D- 10
N135 ;Correcteur armoire
N145 T5
N155 M6
N165 M3 S18000
N175 G0 X-136. Y65.325
N185 Z50.0 M209
N195 Z1.
N205 G1 Z-6.2 F200
N215 G41 Y58.325 F100
N225 G3 X-129. Y65.325 I0. J7.
N235 G3 X-136. Y72.325 I-7. J0.
N245 G3 X-143. Y65.325 I0. J-7.
N255 G3 X-136. Y58.325 I7. J0.
N265 G3 X-135.003 Y58.396 I0. J7.
N275 G1 G40 X-136. Y65.325
N285 G0 Z50.
N295 ;Fin du programme
N305 L Sub_prog_end.nc
N315 M02
