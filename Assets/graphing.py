# -*- coding: utf-8 -*-
"""
Spyder Editor

This is a temporary script file.
"""

import numpy as np
import matplotlib.pyplot as plt
record1=['#1_squarex','#2_squareo']
record2=['#3_squarex','#4_squareo']
record3=['#6_task2','#8_task4','#9_preserve']
record4=['#4_squareo','#9_preserve']
record5=['#10_개샛기들아']

datas=[]
for filename in record4:    #여기
    data=[]
    file=open('record\\'+filename+'.txt')
    l=file.readlines()
    for i in l:
        m=i.split(',')
        data.append([float(m[0]),float(m[1]),float(m[2].strip())])
    datas.append(data)
    
fig = plt.figure(dpi=150)
for data in datas:
    N=np.array(data)
    generation=N[:,0]
    distance=N[:,1]
    varience=N[:,2]
    plt.plot(generation,distance,label=record4[datas.index(data)])#여기
plt.legend(loc='upper right')
plt.show()
plt.close()

fig = plt.figure(dpi=150)
for data in datas:
    N=np.array(data)
    generation=N[:,0]
    varience=N[:,2]
    plt.plot(generation,varience,label=record4[datas.index(data)])#여기
plt.legend(loc='upper right')
plt.show()