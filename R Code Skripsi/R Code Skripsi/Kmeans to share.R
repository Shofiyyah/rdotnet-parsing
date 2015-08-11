library(class)
library(cluster)
#Jika Error loading package, uncomment line 4
#install.packages(c("clv", "fpc"))
library(clv)
library(fpc)

####################################################
############## MY KMEANS GONNA START HERE ##########
####################################################

### INPUT ###
nItter <- 10 #iteration


k <- 3 #centroid
x <- USArrests
y <- read.xlsx("C:/temp/mydata.xls",1)
y
y <- y[2:ncol(y)]
y
### METHOD ###
myKmeans <- function(x, k, nItter) {
  #create random centroid
  centroid_acak <- sample(c(1:length(x[,1])), k, replace = FALSE)
  centroid <- x[centroid_acak,1:length(x)]
  
  #count euclidean distance
  hitung_jarak <- function(x){
    sqrt(
      for(i in 1:length(x)) {
        euclidean <- ((x[i] - centroid[1:k,i])^2)
        euclidean <- euclidean + euclidean
        return(euclidean)
      }
    )   
  }
  
  #for trace
  clusterHistory <- vector(nItter, mode='list')
  centroidHistory <- vector(nItter, mode="list")
  varianceHistory <- vector(nItter, mode="list")
  
  for(i in 1:nItter) {
    jarak <- t(apply(as.matrix(x), 1, hitung_jarak)) #returns matrix of distance
    clusters <- apply(jarak, 1, which.min)           #assign objects to cluster based the shortest distance
    centroid <- apply(x, 2, tapply, clusters, mean)  #update centroid
    Var <- apply(jarak, 2, tapply, clusters, mean)   #Variance with quantization error
    VarQE <- mean(diag(Var))                         #Variance with quantization error
    
    clusterHistory[[i]] <- clusters
    centroidHistory[[i]] <- centroid
    varianceHistory[[i]] <- VarQE
    
    centroid <- centroid
  }
  
  list(clusters=clusterHistory, centroid=centroidHistory, VarQE=varianceHistory)
}



## TEST KMEANS FUNCTION ##
testKmeans <- myKmeans(y, 3, 10)
testKmeans2 <- myKmeans(y, 5, 20)
testKmeans$centroid
testKmeans$clusters
testKmeans$VarQE

### OUTPUT ###
#Output Intracluster, Intercluster, Quantization Error
clsInteger <- as.integer(testKmeans$clusters[[10]])
Validcls <- cls.scatt.data(y,clsInteger)
Intracls <- mean(Validcls$intracls.average) #Intraclass
preInter <- Validcls$intercls.average ###NEW
to.upper <- function(X) X[upper.tri(X,diag=FALSE)] ###NEW
Intercls <- mean(to.upper(preInter))       ###EDITED
QuanzError <- testKmeans$VarQE[[nItter]]                      #Quantization Error
Cluster <- matrix(unlist(testKmeans$clusters[[10]]))
cbind.data.frame(as.matrix(row.names(y)),Cluster)    #cluster result dataview

#Plot Convergence
plot(unlist(testKmeans$VarQE),type="n", main="Convergence", xlab="Iteration", ylab="Quantization Error")
lines(unlist(testKmeans$VarQE),type="o") #Output show up in groupbox convergence

#Plot cluster
plot.new()
clusplot(y,testKmeans$clusters[[10]],main="Cluster Plot",color=TRUE,shade=TRUE, labels=2, lines=0) #Output show up in groupbox cluster plot

