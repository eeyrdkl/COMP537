clear
clc

load('model.mat');
load('data.mat');

%allX = cat(1, trainX, testX);
%allY = cat(2, trainY, testY);
%all 98.33% 177/180

allX = testX;
allY = testY;

%test
%90% 27/30
[predicted_label, accuracy, prob_estimates]=svmpredict(allY', allX, model, '-b 1');