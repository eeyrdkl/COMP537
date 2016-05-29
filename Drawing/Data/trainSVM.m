clear
clc

load('data.mat');


svmType = 'linear';

if strcmp(svmType, 'linear')
    mode = ['-t 0'];
elseif strcmp(svmType, 'poly')
    mode = ['-t 1'];
elseif strcmp(svmType, 'rbf')
    mode = ['-t 2'];
end

model=svmtrain(trainY', trainX, [mode ' -m 1024 -b 1']);


save('model.mat', 'model');