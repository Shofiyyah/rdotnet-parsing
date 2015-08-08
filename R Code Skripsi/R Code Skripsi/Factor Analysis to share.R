#Jika Error loading package, uncomment line 2
#install.packages(c("Cairo"))
library(psych)

### INPUT ###
z <- attitude
f <- 2 #number of factor
z
### OUTPUT ###
fa.parallel(attitude) #Plot Number Of Factor
blah <- capture.output(KMO(cor(attitude)))
print(blah)
#MSA Value if Correlation checked - Output show up in groupbox MSA value
KMO(cov(attitude))    #MSA value if covariance checked - Output show up in groupbox MSA Value
testFA <- principal(attitude, 2, rotate="varimax") #rotate tergantung checkbox 'rotate'
fa.diagram(testFA)    #Factor Diagram - Output show up in groupbox Factor diagram
print (testFA$scores)         #Scoring - Output show up in Factor Scores dataview
print(testFA$communality) #Communality - Output
