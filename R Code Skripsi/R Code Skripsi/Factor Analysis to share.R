#Jika Error loading package, uncomment line 2
#install.packages(c("XLConnect"))
library(psych)
library(xlsx)
library(XLConnect)  

### INPUT ###
z <- attitude
f <- 2 #number of factor

### OUTPUT ###
fa.parallel(attitude) #Plot Number Of Factor
blah <- capture.output(KMO(cor(attitude)))
print(blah)
#MSA Value if Correlation checked - Output show up in groupbox MSA value
KMO(cov(attitude))    #MSA value if covariance checked - Output show up in groupbox MSA Value
testFA <- principal(attitude, 2, rotate="varimax") #rotate tergantung checkbox 'rotate'
fa.diagram(testFA)    #Factor Diagram - Output show up in groupbox Factor diagram
scoring <- testFA$scores        #Scoring - Output show up in Factor Scores dataview
scoring[,2]
commun <- (testFA$communality) #Communality - Output
commun
write.xlsx(commun, "c:/temp/mydata2.xls")

y1 <- iris[1:2]
y2 <- iris[3:4]
y3 <- cbind(y1,y2)
y3
write.xlsx(y1, "c:/temp/mydata.xls")
ydata = read.xlsx("C:/temp/mydata.xls",1)
ydata
