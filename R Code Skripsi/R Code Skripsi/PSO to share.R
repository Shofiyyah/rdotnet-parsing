library(class)
library(cluster)
library(clv)
library(fpc)

####################################################
############## MY PSO GONNA START HERE #############
####################################################

### INPUT ###
#Constants PSO
c1 <- 1.49
c2 <- 1.49
w <- 0.72        
k <- 3
x <- USArrests 
y <- iris[1:4]
nItter <- 101

### METHOD ###
myPSO <- function(x, k, nItter, c1, c2, w) {
  #Create Random centroid
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
  
  
  #Fitness Function Cluster
  OF <- function(x){
    jarak <- t(apply(as.matrix(x), 1, hitung_jarak))  #returns matrix of distance
    clusters <- apply(jarak, 1, which.min)         #assign objects to cluster based the shortest distance
    Var <- apply(jarak, 2, tapply, clusters, mean)   #Variance with quantization error
    VarQE <- mean(diag(Var))                         #Variance with quantization error
    tabclust <- diag(1/table(factor(clusters, levels=1:k)))         #Fitness Function
    quantError <- as.matrix(rowSums(jarak %*% tabclust)/k)          #Fitness Function
    return(quantError)
  }
  
  #Matrix Random Uniform
  mRU <- function(x)
    # returns a matrix of size mxn of uniform random variates
  {
    return(array(runif(k*length(x)), dim = c(k,length(x))))
  }

  #initialize position and velocity
  mP <- x
  mV <- matrix(0,k,length(x))
  
  #count fitness function
  vF <- OF(x)
  
  #Matrix Best Solutions
  mPbest <- mP                 # matrix of `personally best' solutions
  vFbest <- vF                 # vector of OF of best solutions
  sGbest <- min(vFbest)            # scalar: best OF-value
  sgbest <- which.min(vFbest)[1]    # scalar: best solution (counter)
  
  #for trace
  clusterHistory <- vector(nItter, mode="list")
  centroidHistory <- vector(nItter, mode="list")
  varianceHistory <- vector(nItter, mode="list")
  
  for(i in 1:nItter) {
    #update centroid
    mDV <- c1 * mRU(x) * (mPbest[row.names(centroid),] - centroid) + c2 * mRU(x) * (matrix(mPbest[sgbest,]) - centroid)
    mV  <- w * mV + mDV
    centroid  <- centroid + mV
    centroid <- centroid
    
    #count fitness function
    #vF <- OF(x)
    jarak <- t(apply(as.matrix(x), 1, hitung_jarak))  #returns matrix of distance
    clusters <- apply(jarak, 1, which.min)         #assign objects to cluster based the shortest distance
    Var <- apply(jarak, 2, tapply, clusters, mean)   #Variance with quantization error
    VarQE <- mean(diag(Var))                         #Variance with quantization error
    tabclust <- diag(1/table(factor(clusters, levels=1:k)))         #Fitness Function
    quantError <- as.matrix(rowSums(jarak %*% tabclust)/k)          #Fitness Function
    vF <- quantError
    
    # find improvements
    logik <- vF < vFbest        # improved solutions
    mPbest[logik,] <- mP[logik,]
    vFbest[logik] <- vF[logik]
    
    # find best solution
    if (min(vF) < sGbest){
      sGbest <- min(vF)
      sgbest <- which.min(vF)[1]
    }
    
    clusterHistory[[i]] <- clusters
    centroidHistory[[i]] <- centroid
    varianceHistory[[i]] <- VarQE
  }
  
  list(clusters=clusterHistory, centroid=centroidHistory, VarQE=varianceHistory)
}

## TEST PSO FUNCTION ##
testPSO <- myPSO(y,k,101,c1,c2,w)

### OUTPUT ###
#Output Intracluster and Intercluster
clsInteger <- as.integer(testPSO$clusters[[100]])
Validcls <- cls.scatt.data(y,clsInteger)
Intracls <- mean(Validcls$intracls.average) #Output show up in groupbox Validity
preInter <- Validcls$intercls.average ###NEW
to.upper <- function(X) X[upper.tri(X,diag=FALSE)] ###NEW
Intercls <- mean(to.upper(preInter))       ### EDITED:Output shop up in groupbox Validity
print("ASD"+Intercls)
QuanzError <- testPSO$VarQE[[100]]                      #Output shop up in groupbox Validity
Cluster <- matrix(unlist(testPSO$clusters[[100]]))
clusterResultDataView <- cbind.data.frame(y,Cluster)                 #Output show up in cluster result dataview
clusterResultDataView[2:ncol(clusterResultDataView)]
#Plot Convergence
plot(unlist(testPSO$VarQE),type="n", main="Convergence", xlab="Iteration", ylab="Quantization Error")
lines(unlist(testPSO$VarQE),type="o") #Output show up in groupbox convergence

#Plot cluster
plot.new()
clusplot(y,testPSO$clusters[[100]],main="Cluster Plot",color=TRUE,shade=TRUE, labels=2, lines=0) #Output show up in groupbox cluster plot

