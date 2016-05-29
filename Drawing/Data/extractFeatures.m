clear
clc

run('vlfeat-0.9.20/toolbox/vl_setup');

dirs = { 'Class Train' 'Implementation Train'  'Inheritance Train' 'Class Test' 'Implementation Test' 'Inheritance Test'};

trainX = [];
trainY = [];

testX = [];
testY = [];

traini = 1;
testi = 1;

for d=1:length(dirs)
    files = dir(dirs{d});

    for i=1:length(files)
        if files(i).isdir
            continue;
        end
        fname = [dirs{d} '/' files(i).name];

        img = imread(fname);
        img = im2single(img);
        img = imresize(img, [1440 2560]);
        hog = vl_hog(img, 8, 'verbose');
        feature = hog(:);
        
        label = 0;
        if ~isempty(strfind(fname, 'Class'))
            label = 1;
        elseif ~isempty(strfind(fname, 'Implementation'))
            label = 2;
        elseif ~isempty(strfind(fname, 'Inheritance'))
            label = 3;
        end
        
        if ~isempty(strfind(fname, 'Train'))
            trainX(traini,:)=feature;
            trainY(traini)=label;
            
            traini = traini + 1;
        elseif ~isempty(strfind(fname, 'Test'))
            testX(testi,:)=feature;
            testY(testi)=label;
            
            testi = testi + 1;
        end
    end
end
save('data.mat', 'trainX', 'trainY', 'testX', 'testY');


